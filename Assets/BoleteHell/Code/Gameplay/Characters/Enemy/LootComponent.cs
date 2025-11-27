using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.Droppables;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Characters.Enemy
{
    /// <summary>
    /// Marks a character as capable of dropping loot upon death.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthComponent))]
    public class LootComponent : MonoBehaviour
    {
        [SerializeField]
        private DropSettings _dropSettings;
        
        [Inject]
        private IDropManager _dropManager;
        
        private void Awake()
        {
            GetComponent<HealthComponent>().OnDeath += () =>
            {
                _dropManager.DropDroplets(gameObject, _dropSettings.dropletContext);
            };
        }
    }
}