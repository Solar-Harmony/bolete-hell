using BoleteHell.Code.Input;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.InputControllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        [field: SerializeField] 
        private float maxLightIntensity = 5.0f;

        private Rigidbody2D _rb;
        // private Light2D _shipExhaustLight;

        [Inject]
        private IEntityRegistry _entities;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            // _shipExhaustLight = GetComponentInChildren<Light2D>();
        }

        private void FixedUpdate()
        {
            // _shipExhaustLight.intensity = input.IsBoosting ? maxLightIntensity / 2.0f : maxLightIntensity;

            float speedFactor = _entities.GetPlayer().GetComponent<MovementComponent>().MovementSpeed;
            Vector2 inputDir = input.MovementDisplacement.normalized;
            float speed = input.IsBoosting ? 2.0f * speedFactor : speedFactor;
            Vector2 newPosition = transform.position + (Vector3)inputDir * (speed * Time.fixedDeltaTime);

            Vector2 mousePos = input.MousePosition;
            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 lookDir = (mousePos - screenCenter).normalized;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion newRotation = Quaternion.Euler(0, 0, angle);
            
            _rb.MovePositionAndRotation(newPosition, newRotation);
            _rb.linearVelocity = inputDir * speed;
        }
    }
}