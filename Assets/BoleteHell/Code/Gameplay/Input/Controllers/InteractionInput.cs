using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Interactions;
using BoleteHell.Code.Input;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Input.Controllers
{
    public class InteractionInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;

        [Inject]
        private IEntityFinder entityFinder;
        
        [SerializeField]
        private float interactionRadius;

        private void OnEnable()
        {
            input.OnInteract += OnInteract;
        }

        private void OnDisable()
        {
            input.OnInteract -= OnInteract;
        }


        private void OnInteract()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, interactionRadius, LayerMask.GetMask("Interactable"));
            if (colliders.Length == 0) return;
            
            Interactable closestInteraction = null;
            float closestDistance = 9999;
            
            foreach (Collider2D col in colliders)
            {
                if (!col.TryGetComponent(out Interactable interactable)) continue;
                
                if (Vector2.Distance(col.gameObject.transform.position, gameObject.transform.position) < closestDistance )
                {
                    closestInteraction = interactable;
                }
            }
            
            closestInteraction?.Interact(entityFinder.GetPlayer());
        }
    }
}
