using System;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Character
{
    [Serializable]
    public class Energy
    {
        [SerializeField]
        public float maxEnergy = 100f;
        
        [SerializeField]
        [Tooltip("energy/s")]
        public float regenRate = 10f;
        
        [SerializeField]
        public float currentEnergy = 100f;

        public bool CanSpend(float amount) => currentEnergy >= amount;

        public bool Spend(float amount)
        {
            if (!CanSpend(amount)) return false;
            currentEnergy -= amount;
            currentEnergy = Mathf.Max(currentEnergy, 0f);
            return true;
        }

        public void Replenish(float deltaTime)
        {
            currentEnergy += regenRate * deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }
}

