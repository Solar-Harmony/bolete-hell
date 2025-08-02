using System;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Health
{
    [Serializable]
    public class Health : IHealth, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public bool IsInvincible { get; private set; } = false;
        
        [field: SerializeField] 
        public int MaxHealth { get; private set; } = 50;
    
        public int CurrentHealth { get; private set; }
    
        public event Action OnDeath;

        public void TakeDamage(int damageAmount)
        {
            if (IsInvincible)
                return;
            
            CurrentHealth -= damageAmount;

            if (CurrentHealth <= 0)
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
