using System;
using System.Collections.Generic;
using BoleteHell.Rendering.SRP.SunShadows;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Rendering.SRP.Silhouette
{
    public class ObstaclesSilhouetteData : ContextItem 
    {
        public TextureHandle SilhouetteTex;
        public Vector2 BufferOrigin; 
        public Vector2 BufferSize;   
        public float MaxShadowLength;

        public override void Reset() 
        {
            SilhouetteTex = TextureHandle.nullHandle;
        }
    }
    
    public class ObstaclesSilhouettePass : ScriptableRenderPass
    {
        private readonly RenderingLayerMask _renderingLayerMask;
        private readonly Material _overrideMaterial;
        private readonly BoleteRenderingSettings _settings;
        
        public ObstaclesSilhouettePass(BoleteRenderingSettings settings)
        {
            _renderingLayerMask = RenderingLayerMask.GetMask(settings.RenderingLayerMaskName);
            _overrideMaterial = settings.SilhouetteMaterial;
            _settings = settings;
        }
        
        private class PassData
        {
            public RendererListHandle RendererListHandle;
            public Material Mat;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string customPassName = "Silhouette Pass for SDF Obstacles";

            using var builder = renderGraph.AddRasterRenderPass<PassData>(customPassName, out var passData);
  
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            
            Camera camera = cameraData.camera;
            
            float halfWidth = camera.orthographicSize * camera.aspect;
            float halfHeight = camera.orthographicSize;
            Vector2 cameraCenter = camera.transform.position;
            Rect cameraRect = new(cameraCenter.x - halfWidth, cameraCenter.y - halfHeight, halfWidth * 2, halfHeight * 2);
            
            // compute overscan bounds so that all shadow casters are included
            float worldPerPixel = 2 * halfHeight / camera.pixelHeight;
            float worldAOPad = _settings.FakeAORadius * worldPerPixel + 2 * worldPerPixel;
            Vector2 sunDir = _settings.SunShadow.SunDirection;
            float maxLen = _settings.SunShadow.MaxLength;
            Rect bufferRect = new(
                cameraRect.xMin + Mathf.Min(0, sunDir.x) * maxLen - worldAOPad,
                cameraRect.yMin + Mathf.Min(0, sunDir.y) * maxLen - worldAOPad,
                cameraRect.width + (Mathf.Max(0, sunDir.x) - Mathf.Min(0, sunDir.x)) * maxLen + 2 * worldAOPad,
                cameraRect.height + (Mathf.Max(0, sunDir.y) - Mathf.Min(0, sunDir.y)) * maxLen + 2 * worldAOPad
            );

            // dimensions of the slihouette
            float density = camera.pixelHeight / (2 * halfHeight);
            int pixelWidth = Mathf.CeilToInt(bufferRect.width * density);
            int pixelHeight = Mathf.CeilToInt(bufferRect.height * density);
            float scale = Mathf.Min(1f,
                _settings.SunShadow.MaxMaskResolution / (float)Math.Max(pixelWidth, pixelHeight));
            pixelWidth = Mathf.CeilToInt(pixelWidth * scale);
            pixelHeight = Mathf.CeilToInt(pixelHeight * scale);
            
            // snap origin to avoid shimmer
            Vector2 texelWorld = new(bufferRect.width / pixelWidth, bufferRect.height / pixelHeight);
            Vector2 origin = new(
                Mathf.Floor(bufferRect.xMin / texelWorld.x) * texelWorld.x,
                Mathf.Floor(bufferRect.yMin / texelWorld.y) * texelWorld.y);
            Vector2 size = bufferRect.size;
            
            // custom culling 
            camera.TryGetCullingParameters(out var cullingParams);
            Vector2 bufferCenter = origin + size * 0.5f;
            var proj = Matrix4x4.Ortho(-size.x / 2.0f, size.x / 2f, -size.y / 2f, size.y / 2f, -1000, 1000);
            var worldToClip = proj * Matrix4x4
                .TRS(new Vector3(bufferCenter.x, bufferCenter.y, camera.transform.position.z), Quaternion.identity,
                    Vector3.one).inverse;
            cullingParams.isOrthographic = true;
            cullingParams.cullingOptions = CullingOptions.None;
            cullingParams.shadowDistance = 0f;
            // todo: use a culling mask?
 //           cullingParams.cullingMask = (int)_settings.SunShadow.CullingMask;

            var planes = GeometryUtility.CalculateFrustumPlanes(worldToClip);
            for (int i = 0; i < cullingParams.cullingPlaneCount; ++i)
            {
                cullingParams.SetCullingPlane(i, planes[i]);
            }

            CullingResults cullingResults = frameData.Get<CullContextData>().Cull(ref cullingParams);
            
            List<ShaderTagId> shaderTagIds = new() {
                new ShaderTagId("Universal2D"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
            };
            var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, renderingData, cameraData, lightData, SortingCriteria.CommonOpaque);
            drawSettings.overrideMaterial = _overrideMaterial;
            var filteringSettings = new FilteringSettings(RenderQueueRange.all);
            var rendererListParams = new RendererListParams(cullingResults, drawSettings, filteringSettings);
            passData.RendererListHandle = renderGraph.CreateRendererList(rendererListParams);
            passData.Mat = _overrideMaterial;
            builder.UseRendererList(passData.RendererListHandle);
            
            RenderTextureDescriptor silhouetteDesc = new(pixelWidth, pixelHeight, GraphicsFormat.R8_UNorm, 0)
            {
                msaaSamples = 1,
                depthBufferBits = 0,
                useMipMap = false,
                autoGenerateMips = false,
                graphicsFormat = GraphicsFormat.R8_UNorm
            };
            TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, silhouetteDesc, "Obstacles Silhouette", true);
            builder.SetRenderAttachment(destination, 0);
            
            // pass to next pass
            var outputData = frameData.Create<ObstaclesSilhouetteData>();
            outputData.SilhouetteTex = destination;
            outputData.BufferOrigin = origin;
            outputData.BufferSize = size;
            outputData.MaxShadowLength = maxLen;
            
            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                data.Mat.SetMatrix(Shader.PropertyToID("_WorldToBufferClip"), worldToClip);
                context.cmd.ClearRenderTarget(RTClearFlags.Color, Color.clear, 1, 0);
                context.cmd.DrawRendererList(data.RendererListHandle);
            });
        }
    }
}