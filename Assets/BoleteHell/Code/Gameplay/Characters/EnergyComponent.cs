using UnityEngine;

namespace BoleteHell.Code.Gameplay.Characters
{
    [DisallowMultipleComponent]
    public class EnergyComponent : MonoBehaviour 
    {
        [SerializeField]
        public float MaxEnergy = 150f;
        
        [SerializeField]
        [Tooltip("energy/s")]
        public float RegenRate = 20f;
        
        [SerializeField]
        public float CurrentEnergy = 100f;

        public bool CanSpend(float amount) => CurrentEnergy >= amount;

        public bool Spend(float amount)
        {
            if (!CanSpend(amount)) return false;
            CurrentEnergy -= amount;
            CurrentEnergy = Mathf.Max(CurrentEnergy, 0f);
            return true;
        }

        public void Replenish(float deltaTime)
        {
            CurrentEnergy += RegenRate * deltaTime;
            CurrentEnergy = Mathf.Min(CurrentEnergy, MaxEnergy);
        }

        public void GainFixedAmount(float amount)
        {
            CurrentEnergy += amount;
            CurrentEnergy = Mathf.Min(CurrentEnergy, MaxEnergy);
            Debug.Log($"Gained {amount} energy");
        }
        
        public void LoseFixedAmount(float amount)
        {
            CurrentEnergy -= amount;
            CurrentEnergy = Mathf.Max(CurrentEnergy, 0);
        }
    }
}

