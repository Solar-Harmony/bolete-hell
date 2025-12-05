using UnityEngine;

namespace BoleteHell.Gameplay.Characters.Enemy
{
    // I am the swarm.
    public class CreepManager : MonoBehaviour
    {
        private const int _maxRipples = 8;
        
        private static readonly int _corruption = Shader.PropertyToID("_Corruption");
        private static readonly int _rippleData = Shader.PropertyToID("_RippleData");
        private static readonly int _rippleCount = Shader.PropertyToID("_RippleCount");
        private static readonly int _lifetime = Shader.PropertyToID("_RippleLifetime");

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _initialSpreadLevel = 0.5f;
        
        [SerializeField]
        private Transform _rippleTarget;

        [SerializeField]
        private float _rippleSpawnInterval = 0.08f;

        [SerializeField]
        private float _rippleLifetime = 1.5f;
        
        [SerializeField]
        private float _minVelocityForRipple = 0.5f;

        [SerializeField]
        private float _rippleForwardOffset = 1.2f;

        [SerializeField]
        private float _velocitySmoothTime = 0.1f;

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

        public Transform RippleTarget
        {
            get => _rippleTarget;
            set => _rippleTarget = value;
        }

        private Vector4[] _ripples = new Vector4[_maxRipples];
        private int _currentRippleIndex = 0;
        private float _timeSinceLastRipple = 0f;
        private Vector3 _lastPosition;
        private Vector3 _velocity;
        private Vector3 _smoothedVelocity;
        private Vector3 _velocityRef;

        private void Start()
        {
            SpreadLevel = _initialSpreadLevel;
            
            for (int i = 0; i < _maxRipples; i++)
                _ripples[i] = new Vector4(0, 0, -100, 0);

            if (_rippleTarget != null)
                _lastPosition = _rippleTarget.position;
        }

        private void Update()
        {
            if (_rippleTarget == null)
                return;

            Vector3 currentPos = _rippleTarget.position;
            _velocity = (currentPos - _lastPosition) / Time.deltaTime;
            _smoothedVelocity = Vector3.SmoothDamp(_smoothedVelocity, _velocity, ref _velocityRef, _velocitySmoothTime);
            float speed = _velocity.magnitude;
            _lastPosition = currentPos;

            _timeSinceLastRipple += Time.deltaTime;

            float dynamicInterval = Mathf.Lerp(_rippleSpawnInterval * 2f, _rippleSpawnInterval * 0.5f, Mathf.Clamp01(speed / 10f));

            if (speed > _minVelocityForRipple && _timeSinceLastRipple >= dynamicInterval)
            {   
                float intensity = Mathf.Clamp01(0.5f + speed / 6f);
                Vector3 forwardOffset = _smoothedVelocity.normalized * _rippleForwardOffset;
                Vector3 ripplePos = currentPos + forwardOffset;
                _ripples[_currentRippleIndex] = new Vector4(ripplePos.x, ripplePos.y, Time.time, intensity);
                _currentRippleIndex = (_currentRippleIndex + 1) % _maxRipples;
                _timeSinceLastRipple = 0f;
            }

            Shader.SetGlobalVectorArray(_rippleData, _ripples);
            Shader.SetGlobalInt(_rippleCount, _maxRipples);
            Shader.SetGlobalFloat(_lifetime, _rippleLifetime);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SpreadLevel = _applyOutsidePlayMode ? _initialSpreadLevel : 0.0f;
        }
#endif
    }
}