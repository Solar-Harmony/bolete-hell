using System;
using BoleteHell.Gameplay.Characters.Enemy.Factory;
using BoleteHell.Utils.LogFilter;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters
{
    public class HealthComponent : MonoBehaviour
    {
        [field: SerializeField]
        public bool IsInvincible { get; set; } = false;

        [field: SerializeField]
        public int MaxHealth { get; private set; } = 50;

        [Inject]
        private EnemyPool _enemyPool;

        public int CurrentHealth { get; private set; }
        public float Percent => (float)CurrentHealth / MaxHealth;

        public event Action OnDeath;
        public static event Action<GameObject, int> OnDamaged;
        public static event Action<GameObject, int> OnHealed;

        private static readonly LogCategory _logHealth = new("Health", new Color(0.58f, 0.07f, 0f));

        private void OnEnable()
        {
            CurrentHealth = MaxHealth;
        }

        public bool IsDead => CurrentHealth <= 0;
        public void TakeDamage(int damageAmount)
        {
            if (IsInvincible || IsDead)
                return;
            
            CurrentHealth = Math.Max(0, CurrentHealth - damageAmount);
            OnDamaged?.Invoke(gameObject, damageAmount);
            Scribe.Log(_logHealth, $"{gameObject.name} took {damageAmount} damage ({CurrentHealth}hp/{MaxHealth}hp).");

            if (IsDead)
            {
                OnDeath?.Invoke();
                OnDeath = null;

                if (TryGetComponent<ICustomDestroy>(out var poolable))
                {
                    poolable.ReturnToPool();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Heal(int healAmount)
        {
            if (IsDead)
                return;

            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + healAmount);
            Scribe.Log(_logHealth, $"Gained {healAmount} hp");
            OnHealed?.Invoke(gameObject, healAmount);
        }
    }
}
