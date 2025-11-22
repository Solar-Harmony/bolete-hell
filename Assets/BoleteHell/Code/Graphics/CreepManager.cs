using UnityEngine;

namespace BoleteHell.Code.Graphics
{
    public class CreepManager : MonoBehaviour
    {
        private static readonly int _corruption = Shader.PropertyToID("_Corruption");
        
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _initialSpreadLevel = 0.5f;
        
        public void SetCreepSpread(float level)
        {
            Shader.SetGlobalFloat(_corruption, Mathf.Clamp01(level));
        }

        private void Start()
        {
            Shader.SetGlobalFloat(_corruption, _initialSpreadLevel);
        }

        private void OnValidate()
        {
            Shader.SetGlobalFloat(_corruption, _initialSpreadLevel);
        }
    }
}