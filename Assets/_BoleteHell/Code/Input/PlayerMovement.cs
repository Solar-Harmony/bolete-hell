using Graphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Input
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private InputController input;
        
        [field: SerializeField] public float SpeedFactor { get; private set; } = 1.0f;

        [SerializeField] private Light2D shipExhaustLight;

        [field: SerializeField] private float maxLightIntensity = 5.0f;

        private void FixedUpdate()
        {
            shipExhaustLight.intensity = input.IsBoosting ? maxLightIntensity / 2.0f : maxLightIntensity;

            var inputDir = input.GetMovementDisplacement().normalized;
            var speed = input.IsBoosting ? 2.0f * SpeedFactor : SpeedFactor;
            transform.position += (Vector3)inputDir * (speed * Time.fixedDeltaTime);

            var mousePos = input.MousePosition;
            var screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            var lookDir = (mousePos - screenCenter).normalized;
            var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}