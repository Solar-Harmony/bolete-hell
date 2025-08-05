using System;
using UnityEngine;

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
        
        public bool IsDead => CurrentHealth <= 0;

        public void TakeDamage(int damageAmount)
        {
            if (IsInvincible || IsDead)
                return;
            
            CurrentHealth = Math.Max(0, CurrentHealth - damageAmount);

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
