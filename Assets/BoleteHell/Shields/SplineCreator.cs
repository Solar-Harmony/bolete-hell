using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineExtrude),typeof(MeshCollider))]
public class SplineCreator : MonoBehaviour
{
    
    SplineContainer splineContainer;
    SplineExtrude splineExtrude;
    MeshFilter meshFilter;
    Spline spline;
    void Awake()
    {
        splineContainer = gameObject.GetComponent<SplineContainer>();
        splineExtrude = gameObject.GetComponent<SplineExtrude>();
        splineExtrude.Capped = true;
        meshFilter = GetComponent<MeshFilter>();
        spline = splineContainer.Spline;
    }

    public void CreateSpline(List<Vector3> points,float width)
    {
        Debug.Log($"Created a new spline fo {name}");
        splineExtrude.Radius = width;
      

        float3[] float3Array = points.Select(v => new float3(v.x,0,v.y)).ToArray();
        spline.AddRange(float3Array,TangentMode.AutoSmooth);
        
        splineExtrude.Rebuild();
        
    }

    
    // public int GetSplineSegmentFromPos(Vector3 position)
    // {
    //     BezierKnot[] splinePoints = spline.Knots.ToArray();
    //     float minDist = float.MaxValue;
    //     int bestSegment = -1;
    //     for (int i = 0; i < splinePoints.Length - 1; i++) {
    //         BezierKnot a = splinePoints[i];
    //         BezierKnot b = splinePoints[i + 1];
    //         Vector3 closest = ClosestPointOnBezierSegment(a, b, position);
    //         float dist = Vector3.Distance(position, closest);
    //         
    //         if (dist < minDist) {
    //             minDist = dist;
    //             bestSegment = i;
    //         }
    //     }
    //     return bestSegment;
    // }
    //
    // Vector3 ClosestPointOnBezierSegment(BezierKnot k0, BezierKnot k1, Vector3 point) {
    //     Vector3 p0 = k0.Position;
    //     Vector3 p1 =  k0.Position + k0.TangentOut;
    //     Vector3 p2 = k1.Position + k1.TangentIn;
    //     Vector3 p3 = k1.Position;
    //
    //     const int steps = 100;
    //     float minDist = float.MaxValue;
    //     Vector3 closest = Vector3.zero;
    //
    //     for (int i = 0; i <= steps; i++) {
    //         float t = i / (float)steps;
    //         Vector3 curvePoint = EvaluateCubicBezier(p0, p1, p2, p3, t);
    //         float dist = Vector3.SqrMagnitude(curvePoint - point);
    //
    //         if (dist < minDist) {
    //             minDist = dist;
    //             closest = curvePoint;
    //         }
    //     }
    //
    //     return closest;
    // }
    //
    // Vector3 EvaluateCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
    //     float u = 1f - t;
    //     return u * u * u * p0 +
    //            3f * u * u * t * p1 +
    //            3f * u * t * t * p2 +
    //            t * t * t * p3;
    // }
    //
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
