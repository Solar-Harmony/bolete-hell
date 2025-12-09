using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoleteHell.Gameplay.Characters.Enemy
{
    public class CreepManager : MonoBehaviour
    {
        private static readonly int _corruption = Shader.PropertyToID("_Corruption");
        private static readonly int _worldLightDir = Shader.PropertyToID("_WorldLightDir");
        private List<IObjective> objectives = new();

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _initialSpreadLevel = 0.5f;

        [SerializeField]
        private Vector3 _worldLightDirection = new Vector3(0.1f, 0.1f, 0.3f);

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

        public Vector3 WorldLightDirection
        {
            get => _worldLightDirection;
            set
            {
                _worldLightDirection = value;
                Shader.SetGlobalVector(_worldLightDir, _worldLightDirection.normalized);
            }
        }

        private void Start()
        {
            SpreadLevel = _initialSpreadLevel;
            WorldLightDirection = _worldLightDirection;
            objectives = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<IObjective>()
                .ToList();
            foreach (IObjective objective in objectives)
            {
                objective.OnCompleted += ObjectiveCompleted;
            }
        }

        private void ObjectiveCompleted()
        {
            SpreadLevel -= 1f / objectives.Count;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SpreadLevel = _applyOutsidePlayMode ? _initialSpreadLevel : 0.0f;
            if (_applyOutsidePlayMode)
                Shader.SetGlobalVector(_worldLightDir, _worldLightDirection.normalized);
        }
#endif
    }
}