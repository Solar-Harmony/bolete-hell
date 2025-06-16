using _BoleteHell.Code.ProjectileSystem.HitHandler;
using Data.Rays;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} lost {damageAmount} hp \n and now has {currentHealth} hp");
        if(currentHealth <= 0)
            OnDeath();
    }

    public void GainHealth(int healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth += healAmount, currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} gained {healAmount} hp \n and now has {currentHealth} hp");

    }

    private void OnDeath()
    {
        Debug.Log($"{gameObject.name} has died");
    }
}
