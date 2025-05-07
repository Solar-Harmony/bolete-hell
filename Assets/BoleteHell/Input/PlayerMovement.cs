using System;
using System.Collections;
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
        private int health = 100;

        [SerializeField]
        private SpriteRuntimeFragmenter fragmenter;

        [SerializeField]
        private Camera mainCamera;

        [field: SerializeField]
        public float SpeedFactor { get; private set; } = 1.0f;

        [SerializeField]
        private Light2D shipExhaustLight;

        [field: SerializeField]
        private float maxLightIntensity = 5.0f;

        private Dodge dodge;

        private void Awake()
        {
            dodge = GetComponent<Dodge>();
            if (dodge == null)
            {
                Debug.LogError("Dodge component not found on the same GameObject!");
            }
        }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + health);
        }

        private void FixedUpdate()
        {
            dodge.Dodging();
            bool isBoosting = Keyboard.current.shiftKey.isPressed;

            shipExhaustLight.intensity = isBoosting ? maxLightIntensity / 2.0f : maxLightIntensity;

            Vector2 inputDir = input.GetMovementDisplacement().normalized;
            float speed = isBoosting ? 2.0f * SpeedFactor : SpeedFactor;
            transform.position += (Vector3)inputDir * (speed * Time.fixedDeltaTime);

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 lookDir = (mousePos - screenCenter).normalized;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy")) //&& !dodge.isInvincible)
            {
                if (health <= 0)
                    fragmenter.Fragment(other.GetContact(0).normal);

                health -= enemyDamage;
            }
        }

    }
}