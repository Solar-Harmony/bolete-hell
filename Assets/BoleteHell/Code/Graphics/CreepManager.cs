using UnityEngine;

namespace BoleteHell.Code.Graphics
{
    public class CreepManager : MonoBehaviour
    {
        private static readonly int _corruption = Shader.PropertyToID("_Corruption");
        
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _initialSpreadLevel = 0.5f;

        public float SpreadLevel
        {
            get => _spreadLevel;
            set
            {
                _spreadLevel = Mathf.Clamp01(value);
                Shader.SetGlobalFloat(_corruption, _spreadLevel);
            }
        }
        private float _spreadLevel;

        private void Awake()
        {
            _spreadLevel = _initialSpreadLevel;
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