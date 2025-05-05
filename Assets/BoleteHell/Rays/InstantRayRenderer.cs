using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class InstantRayRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer rayRenderer;
    private LineRendererPool parentPool;
    public int Id { get; private set; }

    public void Init(int id)
    {
        Id = id;
    }

    public void DrawRay(List<Vector3> positions, Color color)
    {
        rayRenderer.positionCount = positions.Count;
        rayRenderer.SetPosition(0, positions[0]);
        
        for (var i = 1; i < positions.Count; i++)
        {
            Vector3 pos = positions[i];
            rayRenderer.SetPosition(i, pos);
            rayRenderer.startColor = color;
            rayRenderer.endColor = color;
        }

        //Reset();
    }

    private void Reset()
    {
        Debug.Log($"Reseting lineRenderer {Id}");
        rayRenderer.positionCount = 0;
        gameObject.SetActive(false);
    }
}
