using System;

namespace BoleteHell.Code.Gameplay.Health
{
    public interface IHealth
    {
        public bool IsInvincible { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        
        public event Action OnDeath;
        
        public void TakeDamage(int damageAmount);
    }
}