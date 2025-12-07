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
        [SerializeField]
        private LootTable _lootTable;
        
        [Inject]
        private IDropManager _dropManager;
        
        private void Awake()
        {
            GetComponent<HealthComponent>().OnDeath += () =>
            {
                _dropManager.TryDropLoot(gameObject, _lootTable);
            };
        }
    }
}