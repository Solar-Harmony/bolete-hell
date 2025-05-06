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

        [SerializeField]
        private float dodgeSpeed = 2f;

        [SerializeField]
        private float dodgeDuration = 0.2f;

        [SerializeField]
        private float invincibilityDuration = 2f;

        [SerializeField]
        private float stutterInterval = 0.1f;

        [SerializeField]
        private int stutterCount = 10;

        [SerializeField]
        private float stutterLifetime = 0.5f;

        [SerializeField]
        private Color stutterColor = new Color(1f, 1f, 1f, 0.3f);

        private bool canDodge = true;
        private bool isInvincible = false;
        [SerializeField] SpriteRenderer spriteRenderer;


        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + health);
        }

        private void FixedUpdate()
        {
            Dodge();
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
            if (other.gameObject.CompareTag("Enemy") && !isInvincible)
            {
                if (health <= 0)
                    fragmenter.Fragment(other.GetContact(0).normal);

                health -= enemyDamage;
            }
        }

        private void Dodge()
        {
            if (input.isDodging && canDodge)
            {
                StartCoroutine(dodgeRoutine());
            }
        }

        private IEnumerator dodgeRoutine()
        {
            canDodge = false;
            isInvincible = true;

            // Get the movement direction
            Vector2 moveDir = input.GetMovementDisplacement().normalized;

            // If no movement input, don't dodge
            if (moveDir == Vector2.zero)
            {
                canDodge = true;
                isInvincible = false;
                yield break;
            }

            // Store original position for stutter effect
            Vector3 originalPos = transform.position;

            // Perform the dodge movement
            float elapsedTime = 0f;
            while (elapsedTime < dodgeDuration)
            {
                transform.position += (Vector3)moveDir * (dodgeSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Create stutter effect
            for (int i = 0; i < stutterCount; i++)
            {
                // Create a visual copy of the player
                GameObject stutterCopy = new GameObject("DodgeStutter");
                stutterCopy.transform.position = transform.position;
                stutterCopy.transform.rotation = transform.rotation;
                SpriteRenderer stutterSprite = stutterCopy.AddComponent<SpriteRenderer>();
                stutterSprite.sprite = spriteRenderer.sprite;
                stutterSprite.color = stutterColor;
                stutterSprite.sortingOrder = spriteRenderer.sortingOrder - 1;

                // Destroy the copy after the specified lifetime
                Destroy(stutterCopy, stutterLifetime);

                yield return new WaitForSeconds(stutterInterval);
            }

            // Wait for invincibility to end
            yield return new WaitForSeconds(invincibilityDuration - dodgeDuration);
            isInvincible = false;

            // Wait for remaining cooldown
            yield return new WaitForSeconds(5f - invincibilityDuration);
            canDodge = true;
        }
    }
}