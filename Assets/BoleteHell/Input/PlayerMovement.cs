using System;
using BoleteHell.Graphics;
using BoleteHell.Utils;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace BoleteHell.Input
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private InputController input;

        [SerializeField]
        private int enemyDamage = 5;
        
        [SerializeField]
        private int enemyDamageOverTime = 1;
        
        [SerializeField]
        private int health = 100;

        private void OnGUI()
        {
            // set font siez
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + health);
        }

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

        private bool bStop = false;
        
        private void FixedUpdate()
        {
            var alpha = NormalizedDistanceToCenter(out Vector2 lookDir);
            // float speed = SpeedFactor * speedCurve.Evaluate(alpha);

            Vector2 inputDir = input.GetMovementDisplacement().normalized;
            Vector2 right = new Vector2(lookDir.y, -lookDir.x);
            Vector2 moveDir = inputDir.x * right + inputDir.y * lookDir;
            
            shipExhaustLight.intensity = Mathf.Lerp(0.0f, maxLightIntensity, alpha); 
            
            // get shift pressed
            float speed = Keyboard.current.shiftKey.isPressed ? 2.0f * SpeedFactor : SpeedFactor;
            if (IsMoving)
                rb.MovePosition(rb.position + moveDir * (speed * Time.fixedDeltaTime));
                // rb.linearVelocity = moveDir * speed;

            if (inputDir.y < 0.0f && !bStop)
            {
                rb.linearVelocity = Vector2.zero;
                bStop = true;
            }
            else
            {
                bStop = false;
            }
            
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        [SerializeField]
        private SpriteRuntimeFragmenter fragmenter;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (health <= 0)
                    fragmenter.Fragment(other.GetContact(0).normal);

                health -= enemyDamage;
            }
        }

        // private void OnCollisionStay2D(Collision2D other)
        // {
        //     if (other.gameObject.CompareTag("Enemy"))
        //     {
        //         if (health <= 0)
        //             fragmenter.Fragment(other.GetContact(0).normal);
        //
        //         health -= enemyDamageOverTime;
        //     }
        // }

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