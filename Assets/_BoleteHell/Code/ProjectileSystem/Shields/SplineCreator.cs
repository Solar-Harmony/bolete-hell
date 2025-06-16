using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Shields
{
    [RequireComponent(typeof(SplineExtrude))]
    public class SplineCreator : MonoBehaviour
    {
        public EdgeCollider2D startCap;
        public EdgeCollider2D endCap;

        private CompositeCollider2D _composite;
        private EdgeCollider2D _leftEdge;
        private MeshFilter _meshFilter;
        private EdgeCollider2D _rightEdge;
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

            // Je dois créer 2 edgeColliders enfant du Composite collider pour faire un collider2D qui suit le spline
            LayerMask shieldLayer = LayerMask.NameToLayer("Shield");

            var leftObj = new GameObject("EdgeCollider1");
            leftObj.transform.parent = transform;
            leftObj.layer = shieldLayer;
            _leftEdge = leftObj.AddComponent<EdgeCollider2D>();
            _leftEdge.tag = ShieldTag;
            
            var rightObj = new GameObject("EdgeCollider2");
            rightObj.transform.parent = transform;
            rightObj.layer = shieldLayer;
            _rightEdge = rightObj.AddComponent<EdgeCollider2D>();
            _rightEdge.tag = ShieldTag;

            var startObj = new GameObject("CapCollider1");
            startObj.transform.parent = transform;
            startObj.layer = shieldLayer;
            startCap = startObj.AddComponent<EdgeCollider2D>();
            startCap.tag = ShieldTag;

            var endObj = new GameObject("CapCollider2");
            endObj.transform.parent = transform;
            endObj.layer = shieldLayer;
            endCap = endObj.AddComponent<EdgeCollider2D>();
            endCap.tag = ShieldTag;
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

            _leftEdge.points = leftPoints;
            _rightEdge.points = rightPoints;

            startCap.points = new[] { leftPoints[0], rightPoints[0] };
            endCap.points = new[] { rightPoints[sampleCount - 1], leftPoints[sampleCount - 1] };
        }

        private Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }
    }
}