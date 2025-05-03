using System;
using BoleteHell.Graphics;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Utils;

namespace BoleteHell.Input
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private InputController input;
        
        [SerializeField]
        private Camera mainCamera;

        [field: SerializeField]
        public float SpeedFactor { get; private set; } = 1.0f;

        [SerializeField]
        private AnimationCurve speedCurve;
        
        [SerializeField]
        private Light2D shipExhaustLight;
        
        [field: SerializeField]
        private float maxLightIntensity = 5.0f;
        
        [SerializeField]
        private Rigidbody2D rb;

        public bool IsMoving => input.GetMovementDisplacement() != Vector2.zero;

        private void Start()
        {
            this.AssertNotNull(input);
            this.AssertNotNull(mainCamera);
            this.AssertNotNull(shipExhaustLight);
            this.AssertNotNull(rb);
        }

        private void FixedUpdate()
        {
            Move();
        }
        
        [SerializeField]
        private SpriteRuntimeFragmenter fragmenter;

        private void OnCollisionEnter2D(Collision2D other)
        {
            fragmenter.Fragment(other.GetContact(0).normal);
        }

        private void Move()
        {
            var alpha = NormalizedDistanceToCenter(out Vector2 dir);
            float speed = SpeedFactor * speedCurve.Evaluate(alpha);
            
            shipExhaustLight.intensity = Mathf.Lerp(0.0f, maxLightIntensity, alpha); 
            
            if (IsMoving)
                rb.linearVelocity = dir * speed;
            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        // Maps the mouse position between 0 and 1, where 0 is the center and 1 is the edge of the screen
        private static float NormalizedDistanceToCenter(out Vector2 direction)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            direction = (mousePos - screenCenter).normalized;
            float alpha = (mousePos - screenCenter).magnitude / screenCenter.magnitude;
            return Mathf.Clamp01(alpha);
        }
    }
}