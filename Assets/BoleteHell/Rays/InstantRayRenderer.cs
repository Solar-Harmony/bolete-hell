using System.Collections.Generic;
using UnityEngine;

namespace Lasers
{
    public class InstantRayRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer rayRenderer;
        private LineRendererPool _parentPool;
        public int Id { get; private set; }

        private void Reset()
        {
            Debug.Log($"Reseting lineRenderer {Id}");
            rayRenderer.positionCount = 0;
            gameObject.SetActive(false);
        }

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
                var pos = positions[i];
                rayRenderer.SetPosition(i, pos);
                rayRenderer.startColor = color;
                rayRenderer.endColor = color;
            }
        }
    }
}