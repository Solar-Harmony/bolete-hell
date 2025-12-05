using UnityEngine;

namespace BoleteHell.Gameplay.Characters.Enemy
{
    public class CreepManager : MonoBehaviour
    {
        private static readonly int _corruption = Shader.PropertyToID("_Corruption");

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _initialSpreadLevel = 0.5f;

#if UNITY_EDITOR
        [SerializeField]
        private bool _applyOutsidePlayMode = false;
#endif

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

        private void Start()
        {
            SpreadLevel = _initialSpreadLevel;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SpreadLevel = _applyOutsidePlayMode ? _initialSpreadLevel : 0.0f;
        }
#endif
    }
}