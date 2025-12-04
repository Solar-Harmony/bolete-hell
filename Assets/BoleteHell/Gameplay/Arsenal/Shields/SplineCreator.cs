using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace BoleteHell.Code.Arsenal.Shields
{
    [RequireComponent(typeof(SplineExtrude))]
    public class SplineCreator : MonoBehaviour
    {
        private CompositeCollider2D _composite;
        private PolygonCollider2D _polygonCollider;
        private MeshFilter _meshFilter;
        private Spline _spline;

        private SplineContainer _splineContainer;
        private SplineExtrude _splineExtrude;
        
        private const string ShieldTag = "Shield";

        private void Awake()
        {
            _splineContainer = gameObject.GetComponent<SplineContainer>();
            _splineExtrude = gameObject.GetComponent<SplineExtrude>();
            _splineExtrude.Capped = true;
            _meshFilter = GetComponent<MeshFilter>();
            _spline = _splineContainer.Spline;
            _composite = GetComponent<CompositeCollider2D>();

            LayerMask shieldLayer = LayerMask.NameToLayer("Shield");
            
            var polyCol = new GameObject("Polygon Collider");
            polyCol.transform.parent = transform;
            polyCol.layer = shieldLayer;
            _polygonCollider = polyCol.AddComponent<PolygonCollider2D>();
            _polygonCollider.tag = ShieldTag;
            _polygonCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        }

        private void OnDrawGizmos()
        {
            var mf = GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) return;

            var mesh = mf.sharedMesh;
            var vertices = mesh.vertices;
            var normals = mesh.normals;

            var t = transform;

            Gizmos.color = Color.green;
            for (var i = 0; i < vertices.Length; i++)
            {
                var worldPos = t.TransformPoint(vertices[i]);
                var worldNormal = t.TransformDirection(normals[i]);
                Gizmos.DrawLine(worldPos, worldPos + worldNormal * 0.1f);
            }
        }

        public void CreateSpline(List<Vector3> points, float width)
        {
            _splineExtrude.Radius = width;
            _meshFilter.sharedMesh = new Mesh();


            var float3Array = points.Select(v => new float3(v.x, 0, v.y)).ToArray();
            _spline.AddRange(float3Array);

            _splineExtrude.Rebuild();

            Create2DColliders();
        }

        private void Create2DColliders()
        {
            var sampleCount = (int)Mathf.Ceil(_splineExtrude.SegmentsPerUnit * _spline.GetLength());
            if (sampleCount == 0) return;
            var width = 0.3f / 2;
            var leftPoints = new Vector2[sampleCount];
            var rightPoints = new Vector2[sampleCount];
            var sampledPositions = new Vector3[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                var t = (float)i / (sampleCount - 1);
                sampledPositions[i] = _splineContainer.EvaluatePosition(t);
            }

            for (var i = 0; i < sampleCount; i++)
            {
                Vector2 pos = sampledPositions[i];
                Vector2 offset;

                if (i == 0)
                {
                    //Estimation de la tangente en prennant la direction entre 2 points de la courbe
                    //On doit l'estimer car on a pas la fonction exacte qui forme la courbe donc on ne peut pas get la dérivé d'un point précis
                    var tangent = ((Vector2)sampledPositions[i + 1] - (Vector2)sampledPositions[i]).normalized;
                    offset = Perpendicular(tangent) * width;
                }
                else if (i == sampleCount - 1)
                {
                    var tangent = ((Vector2)sampledPositions[i] - (Vector2)sampledPositions[i - 1]).normalized;
                    offset = Perpendicular(tangent) * width;
                }
                else
                {
                    // For interior points, compute two tangents and their perpendicular normals
                    var tangentA = ((Vector2)sampledPositions[i] - (Vector2)sampledPositions[i - 1]).normalized;
                    var tangentB = ((Vector2)sampledPositions[i + 1] - (Vector2)sampledPositions[i]).normalized;
                    var normalA = Perpendicular(tangentA);
                    var normalB = Perpendicular(tangentB);

                    //Miter join, permet de garder une distance constante pour le width de la ligne même dans des curve
                    //Average the normals (this gives a miter direction)
                    var miter = (normalA + normalB).normalized;
                    
                    var dot = Vector2.Dot(miter, normalA);
                    var miterFactor = dot != 0 ? width / dot : width;
                    offset = miter * miterFactor;
                }

                leftPoints[i] = pos + offset;
                rightPoints[i] = pos - offset;
            }

            //Fix le probleme avec le compositeCollider qui ne reconnait pas ses child colliders
            //j'utilisait mal le compositeCollider je pense
            List<Vector2> polygonPath = new();
            polygonPath.AddRange(leftPoints);
            for (int i = rightPoints.Length - 1; i >= 0; i--)
                polygonPath.Add(rightPoints[i]);

            _polygonCollider.SetPath(0, polygonPath.ToArray());
        }

        private Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }
    }
}