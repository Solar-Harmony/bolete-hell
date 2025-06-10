#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.SDF
{
    // TODO: This is vibe-coded w/ ChatGPT. It works okay but it's a very naive approach lol
    //       Ideally we'd rewrite this with William's help to run on GPU with compute shader
    public static class SDFGeneratorImpl
    {
        class Results
        {
            public float[,] signedDist;
            public int texWidth;
            public int texHeight;
        }

        class SDFParams
        {
            public int baseResolution;
            public float padding; 
            public int blurRadius; 
            public bool subsample;
        }
        
        /// <summary>
        /// Generates a 2D signed-distance-field (SDF) Texture2D from an arbitrary 2D mesh,
        /// using a CPU-based two-pass Euclidean distance transform for exact results, 
        /// with optional sub-pixel rasterization and uniform padding around the mesh bounds.
        /// </summary>
        /// <param name="sdfTexture"></param>
        /// <param name="mesh">A Mesh whose vertices lie in the XY plane (Z = 0).</param>
        /// <param name="baseResolution">
        /// Number of pixels along the longest side of the (padded) mesh’s axis-aligned bounds. 
        /// The shorter side will be scaled so that one texel ≈ one world unit in both axes.
        /// </param>
        /// <param name="padding">
        /// World-space padding to add on each side of the mesh bounds. The bounds will be expanded
        /// by "padding" units in X and Y before rasterization and distance computation.
        /// </param>
        /// <param name="blurRadius">Radius to gaussian blur the thing</param>
        /// <param name="subsample">If true, uses a 2×2 sub-sample grid per pixel for the mask.</param>
        /// <returns>
        /// A Texture2D in RFloat format: the red channel stores signed distances (negative inside, positive outside).
        /// </returns>
        public static async Task<Texture2D> GenerateSDF(
            Mesh mesh,
            int baseResolution,
            float padding,
            int blurRadius,
            bool subsample
        )
        {
            // 1. Project mesh vertices to 2D
            Vector3[] verts3D = mesh.vertices;
            Bounds originalBounds = mesh.bounds;
            int[] tris = mesh.triangles;

            return await Task.Run(() => GenerateSDFAsync(verts3D, originalBounds, tris, baseResolution, padding, blurRadius,
                subsample)).ContinueWith(sdf =>
            {
                Results results = sdf.Result;
                Texture2D sdfTex = new Texture2D(results.texWidth, results.texHeight, TextureFormat.RFloat, false, true);
                sdfTex.wrapMode   = TextureWrapMode.Clamp;
                sdfTex.filterMode = FilterMode.Bilinear;

                for (int py = 0; py < results.texHeight; py++)
                {
                    for (int px = 0; px < results.texWidth; px++)
                    {
                        float d = results.signedDist[px, py];
                        sdfTex.SetPixel(px, py, new Color(d, 0f, 0f, 0f));
                    }
                }
                
                sdfTex.Apply();
                return sdfTex;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static Results GenerateSDFAsync(
            Vector3[] verts3D, 
            Bounds originalBounds, 
            int[] tris,
            int baseResolution,
            float padding,
            int blurRadius,
            bool subsample
        )
        {
            Vector2[] verts2D = new Vector2[verts3D.Length];
            for (int i = 0; i < verts3D.Length; i++)
                verts2D[i] = new Vector2(verts3D[i].x, verts3D[i].y);

            // 2. Compute axis-aligned bounding box (AABB) in local space, then add padding
            Vector2 min = new Vector2(originalBounds.min.x, originalBounds.min.y);
            Vector2 max = new Vector2(originalBounds.max.x, originalBounds.max.y);
            min.x -= padding;
            min.y -= padding;
            max.x += padding;
            max.y += padding;
            float width  = max.x - min.x;
            float height = max.y - min.y;

            // 3. Determine texture dimensions so that world-unit-per-texel is equal in X and Y
            int texWidth, texHeight;
            if (width >= height)
            {
                texWidth  = baseResolution;
                texHeight = Mathf.Max(1, Mathf.CeilToInt(baseResolution * (height / width)));
            }
            else
            {
                texHeight = baseResolution;
                texWidth  = Mathf.Max(1, Mathf.CeilToInt(baseResolution * (width  / height)));
            }

            // 4. Compute world-unit size per texel (they are equal by construction)
            float texelWorldSizeX = width  / texWidth;
            float texelWorldSizeY = height / texHeight;
            float texelSize = 0.5f * (texelWorldSizeX + texelWorldSizeY);

            // 5. Rasterize mesh into a binary mask, with optional 2×2 subsampling
            bool[,] maskInside = new bool[texWidth, texHeight];

            // Precompute triangle data for faster point-in-triangle calls
            (Vector2 a, Vector2 b, Vector2 c)[] triData = new (Vector2, Vector2, Vector2)[tris.Length / 3];
            for (int t = 0; t < tris.Length; t += 3)
            {
                triData[t / 3] = (
                    verts2D[tris[t + 0]],
                    verts2D[tris[t + 1]],
                    verts2D[tris[t + 2]]
                );
            }

            // Subsample offsets (2×2 grid)
            Vector2[] subOffsets = subsample
                ? new Vector2[] {
                    new Vector2(-0.25f, -0.25f),
                    new Vector2( 0.25f, -0.25f),
                    new Vector2(-0.25f,  0.25f),
                    new Vector2( 0.25f,  0.25f)
                }
                : new Vector2[] { Vector2.zero };

            for (int py = 0; py < texHeight; py++)
            {
                for (int px = 0; px < texWidth; px++)
                {
                    bool anySubInside = false;
                    foreach (var offset in subOffsets)
                    {
                        float uSub = (px + 0.5f + offset.x) / texWidth;
                        float vSub = (py + 0.5f + offset.y) / texHeight;
                        Vector2 sample = new Vector2(
                            min.x + uSub * width,
                            min.y + vSub * height
                        );

                        for (int ti = 0; ti < triData.Length; ti++)
                        {
                            var tri = triData[ti];
                            if (PointInTriangle(sample, tri.a, tri.b, tri.c))
                            {
                                anySubInside = true;
                                break;
                            }
                        }
                        if (anySubInside)
                            break;
                    }
                    maskInside[px, py] = anySubInside;
                }
            }

            // 6. Prepare grids for two-pass Euclidean distance transform
            float INF = texWidth * texWidth + texHeight * texHeight;
            float[,] fInside  = new float[texWidth, texHeight];
            float[,] fOutside = new float[texWidth, texHeight];

            for (int py = 0; py < texHeight; py++)
            {
                for (int px = 0; px < texWidth; px++)
                {
                    if (maskInside[px, py])
                    {
                        fInside[px, py]  = 0f;
                        fOutside[px, py] = INF;
                    }
                    else
                    {
                        fInside[px, py]  = INF;
                        fOutside[px, py] = 0f;
                    }
                }
            }

            // 7. Run 2D Euclidean distance transforms
            float[,] gInside  = DistanceTransform2D(fInside,  texWidth, texHeight);
            float[,] gOutside = DistanceTransform2D(fOutside, texWidth, texHeight);

            // 8. Convert to world-space distances
            float[,] distToInside  = new float[texWidth, texHeight];
            float[,] distToOutside = new float[texWidth, texHeight];
            for (int py = 0; py < texHeight; py++)
            {
                for (int px = 0; px < texWidth; px++)
                {
                    distToInside[px, py]  = Mathf.Sqrt(gInside[px, py])  * texelSize;
                    distToOutside[px, py] = Mathf.Sqrt(gOutside[px, py]) * texelSize;
                }
            }

            // 9. Combine into signed distances
            float[,] signedDist = new float[texWidth, texHeight];
            for (int py = 0; py < texHeight; py++)
            {
                for (int px = 0; px < texWidth; px++)
                {
                    signedDist[px, py] = maskInside[px, py]
                        ? -distToOutside[px, py]
                        :  distToInside[px, py];
                }
            }

            // 10. Apply Gaussian blur to smooth the SDF
            GaussianBlur2D(signedDist, texWidth, texHeight, blurRadius, 1.0f);

            return new Results
            {
                signedDist = signedDist,
                texWidth   = texWidth,
                texHeight  = texHeight
            };
        }

        /// <summary>
        /// 2D Euclidean Distance Transform (Felzenszwalb & Huttenlocher).
        /// Input: f[x,y] is 0 at “object” pixels, and large at “background.” 
        /// Output: g[x,y] = min_{(i,j)}[ (i−x)^2 + (j−y)^2 + f[i,j] ].
        /// </summary>
        private static float[,] DistanceTransform2D(float[,] f, int width, int height)
        {
            float[,] dtRows = new float[width, height];
            float[,] dt     = new float[width, height];
            float[] fcol    = new float[Mathf.Max(width, height)];
            float[] dcol    = new float[Mathf.Max(width, height)];

            // Transform along columns
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    fcol[y] = f[x, y];
                DistanceTransform1D(fcol, dcol, height);
                for (int y = 0; y < height; y++)
                    dtRows[x, y] = dcol[y];
            }

            // Transform along rows
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    fcol[x] = dtRows[x, y];
                DistanceTransform1D(fcol, dcol, width);
                for (int x = 0; x < width; x++)
                    dt[x, y] = dcol[x];
            }

            return dt;
        }

        /// <summary>
        /// 1D Euclidean Distance Transform (Felzenszwalb algorithm).
        /// Computes d[i] = min_j[ (i−j)^2 + f[j] ] in O(n).
        /// </summary>
        private static void DistanceTransform1D(float[] f, float[] d, int n)
        {
            int[] v = new int[n];
            float[] z = new float[n + 1];
            int k = 0;
            v[0] = 0;
            z[0] = float.NegativeInfinity;
            z[1] = float.PositiveInfinity;

            for (int q = 1; q < n; q++)
            {
                float s;
                while (true)
                {
                    int p = v[k];
                    s = ((f[q] + q*q) - (f[p] + p*p)) / (2f * (q - p));
                    if (s <= z[k])
                        k--;
                    else
                        break;
                }
                int pFinal = v[k];
                float sNew = ((f[q] + q*q) - (f[pFinal] + pFinal*pFinal)) / (2f * (q - pFinal));
                k++;
                v[k] = q;
                z[k] = sNew;
                z[k + 1] = float.PositiveInfinity;
            }

            k = 0;
            for (int q = 0; q < n; q++)
            {
                while (z[k + 1] < q) k++;
                int p = v[k];
                d[q] = (q - p)*(q - p) + f[p];
            }
        }

        /// <summary>
        /// Barycentric / area-based 2D point-in-triangle test.
        /// Returns true if p lies inside or on the boundary of triangle ABC.
        /// </summary>
        private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 v0 = c - a;
            Vector2 v1 = b - a;
            Vector2 v2 = p - a;

            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            float denom = dot00 * dot11 - dot01 * dot01;
            if (Mathf.Approximately(denom, 0f))
                return false;

            float invDenom = 1f / denom;
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return (u >= 0f) && (v >= 0f) && (u + v <= 1f);
        }
        
        /// <summary>
        /// Blurs (in place) a 2D array of signed‐distance floats with a separable Gaussian kernel.
        /// </summary>
        /// <param name="data">Input SDF array of size [width, height]. On return, this will be the blurred result.</param>
        /// <param name="width">Texture width (number of columns).</param>
        /// <param name="height">Texture height (number of rows).</param>
        /// <param name="radius">
        /// Radius of the Gaussian kernel in pixels. Typical values: 1–3. Larger radii = more blur.
        /// </param>
        /// <param name="sigma">
        /// Standard deviation of the Gaussian. If you set sigma = radius/2, the kernel will cover ≈ 99% of its area.
        /// </param>
        private static void GaussianBlur2D(
            float[,] data,
            int width,
            int height,
            int radius,
            float sigma
        )
        {
            // 1) Build 1D Gaussian kernel (length = 2 * radius + 1)
            int kernelSize = 2 * radius + 1;
            float[] kernel = new float[kernelSize];
            float twoSigmaSq = 2f * sigma * sigma;
            float invRoot = 1f / (Mathf.Sqrt(Mathf.PI * twoSigmaSq));
            float sum = 0f;

            for (int i = 0; i < kernelSize; i++)
            {
                int x = i - radius;
                float value = invRoot * Mathf.Exp(-(x * x) / twoSigmaSq);
                kernel[i] = value;
                sum += value;
            }

            // Normalize kernel so it sums to 1
            for (int i = 0; i < kernelSize; i++)
                kernel[i] /= sum;

            // 2) Temporary buffer for horizontal pass
            float[,] temp = new float[width, height];

            // Horizontal blur pass
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float accum = 0f;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int sampleX = Mathf.Clamp(x + k, 0, width - 1);
                        accum += data[sampleX, y] * kernel[k + radius];
                    }
                    temp[x, y] = accum;
                }
            }

            // 3) Vertical blur pass (write back into data)
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float accum = 0f;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int sampleY = Mathf.Clamp(y + k, 0, height - 1);
                        accum += temp[x, sampleY] * kernel[k + radius];
                    }
                    data[x, y] = accum;
                }
            }
        }
    }
}
#endif