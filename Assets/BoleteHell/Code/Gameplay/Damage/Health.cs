using System;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage
{
    public class Health : MonoBehaviour, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public bool IsInvincible { get; set; } = false;
        
        [field: SerializeField] 
        public int MaxHealth { get; private set; } = 50;
    
        public int CurrentHealth { get; private set; }
        
        public float Percent => (float)CurrentHealth / MaxHealth;
    
        public event Action OnDeath;
        public static event Action<GameObject, int> OnDamaged;
        public static event Action<GameObject, int> OnHealed;

        public bool IsDead => CurrentHealth <= 0;
        public void TakeDamage(int damageAmount)
        {
            if (IsInvincible || IsDead)
                return;
            
            CurrentHealth = Math.Max(0, CurrentHealth - damageAmount);
            OnDamaged?.Invoke(gameObject, damageAmount);

            if (IsDead)
            {
                OnDeath?.Invoke();
                OnDeath = null;
            }
        }
        
        public void Heal(int healAmount)
        {
            if(IsDead)return;
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + healAmount);
            OnHealed?.Invoke(gameObject, healAmount);
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
