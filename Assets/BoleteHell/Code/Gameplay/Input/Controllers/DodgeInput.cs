using System.Collections;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Input.Controllers
{
    public class DodgeInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;

        [SerializeField]
        private int maxNumberOfDodges;

        [SerializeField]
        private float dodgeSpeed = 2f;

        [SerializeField]
        private float dodgeDuration = 0.2f;

        [SerializeField]
        private float invincibilityDuration = 2f;

        [SerializeField]
        private float dodgeRechargeTimer = 3f;
        
        [SerializeField]
        private float stutterInterval = 0.1f;

        [SerializeField]
        private int stutterCount = 10;

        [SerializeField]
        private float stutterLifetime = 0.5f;

        [SerializeField]
        private Color stutterColor = new Color(1f, 1f, 1f, 0.3f);

        [SerializeField]
        SpriteRenderer spriteRenderer;

        private int remainingDodges;

        private HealthComponent health;

        private void OnEnable()
        {
            input.OnDodge += Dodging;
        }

        private void OnDisable()
        {
            input.OnDodge -= Dodging;

        }

        private void Start()
        {
            remainingDodges = maxNumberOfDodges;
            health = GetComponent<HealthComponent>();
        }

        private void Dodging()
        {
            if (remainingDodges > 0)
            {
                //Permet d'ignorer les ennemis quand on dodge donc on est pas bloquer et on peut plus facilement aller backstab les ennemis
                gameObject.layer = LayerMask.NameToLayer($"PlayerDodge");
                StartCoroutine(DodgingRoutine());
            }
        }

        private IEnumerator DodgingRoutine()
        {
            health.IsInvincible = true;

            // Get the movement direction from PlayerMovement's current input
            Vector2 moveDir = input.MovementDisplacement.normalized;

            // If no movement input, don't dodge
            if (moveDir == Vector2.zero)
            {
                gameObject.layer = LayerMask.NameToLayer($"Unit");
                health.IsInvincible = false;
                yield break;
            }

            remainingDodges--;
            StartCoroutine(StutterRoutine());
            
            // Perform the dodge movement
            float elapsedTime = 0f;
            while (elapsedTime < dodgeDuration)
            {
                transform.position += (Vector3)moveDir * (dodgeSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Wait for invincibility to end
            yield return new WaitForSeconds(invincibilityDuration - dodgeDuration);
            health.IsInvincible = false;
            gameObject.layer = LayerMask.NameToLayer($"Unit");

            // Wait for remaining cooldown
            yield return new WaitForSeconds(dodgeRechargeTimer - invincibilityDuration);
            remainingDodges++;
        }
        
        //This can't possibly be good performance wise
        private IEnumerator StutterRoutine()
        {
            for (int i = 0; i < stutterCount; i++)
            {
                GameObject stutterCopy = new GameObject("DodgeStutter");
                stutterCopy.transform.position = transform.position;
                stutterCopy.transform.rotation = transform.rotation;
                SpriteRenderer stutterSprite = stutterCopy.AddComponent<SpriteRenderer>();
                stutterSprite.sprite = spriteRenderer.sprite;
                stutterSprite.color = stutterColor;
                stutterSprite.sortingOrder = spriteRenderer.sortingOrder - 1;

                Destroy(stutterCopy, stutterLifetime);

                yield return new WaitForSeconds(stutterInterval);
            }
        }

    }
}
