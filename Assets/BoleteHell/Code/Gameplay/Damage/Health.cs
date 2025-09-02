using System;
using UnityEngine;
using UnityEngine.Events;

namespace BoleteHell.Code.Gameplay.Damage
{
    [Serializable]
    public class Health : ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public bool IsInvincible { get; private set; } = false;
        
        [field: SerializeField] 
        public int MaxHealth { get; private set; } = 50;
    
        public int CurrentHealth { get; private set; }
    
        public event Action OnDeath;

        public static UnityAction<GameObject, int> onDamaged; ////NIng basically event

        public bool IsDead => CurrentHealth <= 0;

        public void TakeDamage(int damageAmount)
        {
            if (IsInvincible || IsDead)
                return;
            
            CurrentHealth = Math.Max(0, CurrentHealth - damageAmount);
            onDamaged?.Invoke(null, damageAmount); ////NIngstuff

            if (IsDead)
            {
                OnDeath?.Invoke();
                OnDeath = null;
            }
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            CurrentHealth = MaxHealth;
        }
    }
}
