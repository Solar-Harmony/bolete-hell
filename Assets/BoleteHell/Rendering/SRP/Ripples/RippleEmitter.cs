using UnityEngine;
using Zenject;

namespace BoleteHell.Rendering.Ripples
{
    public class RippleEmitter : MonoBehaviour
    {
        [SerializeField]
        private float _rippleSpawnInterval = 0.08f;

        [SerializeField]
        private float _minVelocityForRipple = 0.5f;

        [SerializeField]
        private float _rippleForwardOffset = 1.2f;

        [SerializeField]
        private float _velocitySmoothTime = 0.1f;

        [Inject]
        private RippleManager _rippleManager;

        private float _timeSinceLastRipple;
        private Vector3 _lastPosition;
        private Vector3 _velocity;
        private Vector3 _smoothedVelocity;
        private Vector3 _velocityRef;

        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 currentPos = transform.position;
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

                _rippleManager?.EmitRipple(new Vector2(ripplePos.x, ripplePos.y), intensity);
                _timeSinceLastRipple = 0f;
            }
        }
    }
}

