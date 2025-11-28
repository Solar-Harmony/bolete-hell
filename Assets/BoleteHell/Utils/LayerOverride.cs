using UnityEngine;

namespace BoleteHell.Code.Gameplay.Graphics
{
    public class LayerOverride : MonoBehaviour
    {
        public int LayerOffset = 0;

        private void Awake()
        {
            Apply();   
        }

        private void OnValidate()
        { 
            Apply(); 
        }

        private void Apply()
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, -LayerOffset);
        }
    }
}