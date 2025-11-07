using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Droppables
{
    public class Droplet : MonoBehaviour
    {
        [field:SerializeReference] [Required]
        private StatusEffectConfig effect;
    
        [SerializeField]
        private float pickupDistance = 2f;
    
        [SerializeField]
        private float speed = 5f; 
    
        [Inject]
        private IStatusEffectService _statusEffectService;

        [Inject]
        private IEntityFinder _entityFinder;

        private Player player;
        private void Start()
        {
            player = _entityFinder.GetPlayer();
        }

        private void Update()
        {
            if (!player) return;
            Vector3 playerPos = player.transform.position;
            if(Vector2.Distance(playerPos, transform.position) > pickupDistance) return;
        
            Vector3 direction = (playerPos - transform.position).normalized;
            transform.position += direction * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
        
            _statusEffectService.AddStatusEffect(player, effect); 
            Destroy(this.gameObject);
        }
    }
}
