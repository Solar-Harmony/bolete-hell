using System;
using BoleteHell.Code.Utils.LogFilter;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage
{
    public class Health : MonoBehaviour, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public bool IsInvincible { get; private set; } = false;
        
        [field: SerializeField] 
        public int MaxHealth { get; private set; } = 50;
    
        public int CurrentHealth { get; private set; }
        
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
            Scribe.Log(LogCategories.Health, $"{gameObject.name} took {damageAmount} damage ({CurrentHealth}hp/{MaxHealth}hp).");

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
            Debug.Log($"Gained {healAmount} hp");
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
