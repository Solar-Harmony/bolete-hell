using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters.Registry;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Droppables
{
    public class Droplet : MonoBehaviour
    {
        [field:SerializeReference] [Required]
        private StatusEffectConfig _effect;
    
        [SerializeField]
        private float _pickupDistance = 2f;
    
        [SerializeField]
        private float _speed = 5f; 
    
        [Inject]
        private IStatusEffectService _statusEffectService;

        [Inject]
        private IEntityRegistry _entities;

        private GameObject _player;
        private void Start()
        {
            _player = _entities.GetPlayer();
        }

        private void Update()
        {
            if (!_player) return;
            Vector3 playerPos = _player.transform.position;
            if(Vector2.Distance(playerPos, transform.position) > _pickupDistance) return;
        
            Vector3 direction = (playerPos - transform.position).normalized;
            transform.position += direction * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
        
            _statusEffectService.AddStatusEffect(_player, _effect); 
            Destroy(this.gameObject);
        }
    }
}
