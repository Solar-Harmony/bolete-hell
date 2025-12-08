using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.Droppables;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Enemy
{
    /// <summary>
    /// Marks a character as capable of dropping loot upon death.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthComponent))]
    public class LootComponent : MonoBehaviour
    {
        [Inject] private IDropManager _dropManager;
        [Inject] private IEntityRegistry _entities;

        [SerializeField] private LootTable _lootTable;
        [SerializeField] private GameObject _healthDroplet;
        [SerializeField] private GameObject _energyDroplet;
        
        private void Awake()
        {
            var player = _entities.GetPlayer();
            var health = player.GetComponent<HealthComponent>();
            var energy = player.GetComponent<EnergyComponent>();
            
            GetComponent<HealthComponent>().OnDeath += () =>
            {
                // TODO: Pour l'instant on hardcode quel droplet utiliser ici, dans l'idéal faudrait faire un système plus
                // générique mais j'avais pas trop d'idée de comment m'y prendre et on a pas le temps
                GameObject droplet = health.Percent < energy.Percent ? _healthDroplet : _energyDroplet;
                _dropManager.Drop(gameObject, droplet, _lootTable);
            };
        }
    }
}