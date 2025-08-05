using System.Collections;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Input.Controllers
{
    public class DodgeInput : MonoBehaviour
    {
        private void Update()
        {
            Dodging();
        }

        [SerializeField] 
        private MovementInput movementInput;
        
        [Inject] 
        private IInputDispatcher input;

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
        public bool isInvincible = false;
        [SerializeField]
        SpriteRenderer spriteRenderer;

        public void Dodging()
        {
            Debug.Log($"Dodge check - isDodging: {input.IsDodging}, canDodge: {canDodge}");
            if (input.IsDodging && canDodge)
            {
                StartCoroutine(dodgingRoutine());
                Debug.Log("Dodge started");
            }
        }

        private IEnumerator dodgingRoutine()
        {
            canDodge = false;
            isInvincible = true;

            // Get the movement direction from PlayerMovement's current input
            Vector2 moveDir = input.MovementDisplacement.normalized;

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
