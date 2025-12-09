using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Gameplay.Characters
{
    [DisallowMultipleComponent]
    public class EnergyComponent : MonoBehaviour 
    {
        [SerializeField] public float MaxEnergy = 150f;
        [SerializeField] public float CurrentEnergy = 100f;
        [SuffixLabel("energy/s")]
        [SerializeField] public float RegenRate = 20f;
        
        public float Percent => CurrentEnergy / MaxEnergy;
        
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
        }
        
        public void LoseFixedAmount(float amount)
        {
            CurrentEnergy -= amount;
            CurrentEnergy = Mathf.Max(CurrentEnergy, 0);
        }
    }
}

