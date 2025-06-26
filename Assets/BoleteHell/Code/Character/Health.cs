using System;
using UnityEngine;

namespace BoleteHell.Code.Character
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private bool isInvincible = false;
        [field: SerializeField] 
        public int MaxHealth { get; private set; } = 50;
    
        public int CurrentHealth { get; private set; }
    
        public event Action OnDeath;

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            if (isInvincible) return;
            CurrentHealth -= damageAmount;

            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
                OnDeath = null;
            }
        }

        public void GainHealth(int healAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth += healAmount, CurrentHealth, MaxHealth);
            Debug.Log($"{gameObject.name} gained {healAmount} hp \n and now has {CurrentHealth} hp");
        }
    }
}
