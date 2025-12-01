using BoleteHell.Gameplay.Characters;
using NUnit.Framework;

namespace Tests
{
    public class HealthTests
    {
        [Test]
        public void TakeDamage_ReducesCurrentHealth()
        {
            var health = new HealthComponent();
            health.OnAfterDeserialize(); // Initializes CurrentHealth to MaxHealth
            int initialHealth = health.CurrentHealth;
            health.TakeDamage(10);
            Assert.AreEqual(initialHealth - 10, health.CurrentHealth);
        }

        [Test]
        public void TakeDamage_TriggersOnDeath_WhenHealthZeroOrBelow()
        {
            var health = new HealthComponent();
            health.OnAfterDeserialize();
            bool died = false;
            health.OnDeath += () => died = true;
            health.TakeDamage(health.CurrentHealth); // Should reduce to 0
            Assert.IsTrue(died);
        }
    }
}

