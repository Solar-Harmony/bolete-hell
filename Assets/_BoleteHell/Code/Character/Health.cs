using System;
using UnityEngine;

namespace _BoleteHell.Code.Character
{
    public class Health : MonoBehaviour
    {
        [SerializeField] 
        private int maxHealth = 50;
    
        public int CurrentHealth { get; private set; }
    
        public event Action OnDeath;

        private void Start()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            CurrentHealth -= damageAmount;

            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
                OnDeath = null;
            }
        }

        public void GainHealth(int healAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth += healAmount, CurrentHealth, maxHealth);
            Debug.Log($"{gameObject.name} gained {healAmount} hp \n and now has {CurrentHealth} hp");
        }
    }
}
