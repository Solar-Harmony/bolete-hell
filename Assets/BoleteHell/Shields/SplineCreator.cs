using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineExtrude))]
public class SplineCreator : MonoBehaviour
{
    
    SplineContainer splineContainer;
    SplineExtrude splineExtrude;
    MeshFilter meshFilter;
    Spline spline;

    private CompositeCollider2D composite;
    private EdgeCollider2D leftEdge;
    private EdgeCollider2D rightEdge;
    public EdgeCollider2D startCap;
    public EdgeCollider2D endCap;
    void Awake()
    {
        splineContainer = gameObject.GetComponent<SplineContainer>();
        splineExtrude = gameObject.GetComponent<SplineExtrude>();
        splineExtrude.Capped = true;
        meshFilter = GetComponent<MeshFilter>();
        spline = splineContainer.Spline;
        composite = GetComponent<CompositeCollider2D>();
        
        //Je doit créé 2 edgeColliders enfant du Composite collider pour faire un collider2D qui suis le spline
        GameObject leftObj = new GameObject("EdgeCollider1");
        leftObj.transform.parent = transform;
        leftEdge = leftObj.AddComponent<EdgeCollider2D>();
        
        GameObject rightObj = new GameObject("EdgeCollider2");
        rightObj.transform.parent = transform;
        rightEdge = rightObj.AddComponent<EdgeCollider2D>();
        
        GameObject startObj = new GameObject("CapCollider1");
        startObj.transform.parent = transform;
        startCap = startObj.AddComponent<EdgeCollider2D>();
        
        GameObject endObj = new GameObject("CapCollider2");
        endObj.transform.parent = transform;
        endCap = endObj.AddComponent<EdgeCollider2D>();
    }

    public void CreateSpline(List<Vector3> points,float width)
    {
        splineExtrude.Radius = width;
      

        float3[] float3Array = points.Select(v => new float3(v.x,0,v.y)).ToArray();
        spline.AddRange(float3Array,TangentMode.AutoSmooth);
        
        splineExtrude.Rebuild();
        
        Create2DColliders();
    }

	private void Create2DColliders()
    {
        int sampleCount = (int)Mathf.Ceil(splineExtrude.SegmentsPerUnit * spline.GetLength()) ;
        float width = 0.3f/2;
        Vector2[] leftPoints = new Vector2[sampleCount];
        Vector2[] rightPoints = new Vector2[sampleCount];
        Vector3[] sampledPositions = new Vector3[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / (sampleCount - 1);
            sampledPositions[i] = splineContainer.EvaluatePosition(t);
        }

        for (int i = 0; i < sampleCount; i++)
        {
            Vector2 pos = sampledPositions[i];
            Vector2 offset;
            
            if (i == 0)
            {
                Vector2 tangent = ((Vector2)sampledPositions[i + 1] - (Vector2)sampledPositions[i]).normalized;
                offset = Perpendicular(tangent) * width;
            }
            else if (i == sampleCount - 1)
            {
                Vector2 tangent = ((Vector2)sampledPositions[i] - (Vector2)sampledPositions[i - 1]).normalized;
                offset = Perpendicular(tangent) * width;
            }
            else
            {
                // For interior points, compute two tangents and their perpendicular normals
                Vector2 tangentA = ((Vector2)sampledPositions[i] - (Vector2)sampledPositions[i - 1]).normalized;
                Vector2 tangentB = ((Vector2)sampledPositions[i + 1] - (Vector2)sampledPositions[i]).normalized;
                Vector2 normalA = Perpendicular(tangentA);
                Vector2 normalB = Perpendicular(tangentB);

                //Miter join, permet de garder une distance constante pour le width de la ligne même dans des curve
                // Average the normals (this gives a miter direction)
                Vector2 miter = (normalA + normalB).normalized;
                

                float dot = Vector2.Dot(miter, normalA);
                float miterFactor = (dot != 0) ? width / dot : width;
                offset = miter * miterFactor;
            }

            leftPoints[i] = pos + offset;
            rightPoints[i] = pos - offset;
        }

        leftEdge.points = leftPoints;
        rightEdge.points = rightPoints;
        
        startCap.points = new[] { leftPoints[0], rightPoints[0] };
        endCap.points = new[] { rightPoints[sampleCount - 1], leftPoints[sampleCount - 1] };
    }
    
    // Helper function to get a 2D perpendicular.
    Vector2 Perpendicular(Vector2 v)
    {
        return new Vector2(-v.y, v.x);
    }

    void OnDrawGizmos() {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        Transform t = transform;

        Gizmos.color = Color.green;
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 worldPos = t.TransformPoint(vertices[i]);
            Vector3 worldNormal = t.TransformDirection(normals[i]);
            Gizmos.DrawLine(worldPos, worldPos + worldNormal * 0.1f);
        }
    }

}
