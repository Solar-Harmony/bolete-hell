using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace BoleteHell.Code.Input.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        [field: SerializeField] 
        public float SpeedFactor { get; private set; } = 1.0f;
        
        [field: SerializeField] 
        private float maxLightIntensity = 5.0f;

        private Rigidbody2D _rb;
        private Light2D _shipExhaustLight;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _shipExhaustLight = GetComponentInChildren<Light2D>();
        }

        private void FixedUpdate()
        {
            _shipExhaustLight.intensity = input.IsBoosting ? maxLightIntensity / 2.0f : maxLightIntensity;

            var inputDir = input.MovementDisplacement.normalized;
            var speed = input.IsBoosting ? 2.0f * SpeedFactor : SpeedFactor;
            Vector2 newPosition = transform.position + (Vector3)inputDir * (speed * Time.fixedDeltaTime);

            var mousePos = input.MousePosition;
            var screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            var lookDir = (mousePos - screenCenter).normalized;
            var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion newRotation = Quaternion.Euler(0, 0, angle);
            
            _rb.MovePositionAndRotation(newPosition, newRotation);
            _rb.linearVelocity = inputDir * speed;
        }
    }
}