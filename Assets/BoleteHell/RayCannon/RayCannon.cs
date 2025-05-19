using System;
using System.Collections.Generic;
using Lasers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prisms
{
    //Automatic: Plein de petit laser
    //Charged: Charge puis tire un laser instant
    //Constant: tire un laser de manière constante
    public enum FiringTypes
    {
        Automatic,
        Charged,
        Constant
    }

    public class RayCannon : MonoBehaviour
    {
        [Tooltip("If the weapon is unlocked by default for the player")]
        [field: SerializeField] public bool IsDefault { get; private set; }
        //Permet de changer de firinType at runtime et d'avoir des stats spécifique a chaque firing type pour un weapon
        [SerializeReference] private List<RayCannonFiringLogic> firingLogics;
        
        private int currentTypeIndex = 0;
        
        //TODO: Add logic to be able to combine rays (not spawn two rays at the same time really make 1 ray have the effects of two different rays (buffs included))
        //maybe combining rays adds logic to the first prisms ray
        //so when you combine two rays you choose one prism in which two rays's logic is combined
        //Or we actually combine the two prisms and average the stats and add the ray logic to the ray
        //Should check if we want to be able to de-combine the prisms after too
        // FIXME: This does not belong here. RayCannon's responsability should be to fire a ray, not to manage the firing logic.
        public void SwitchFiringType()
        {
            if (firingLogics.Count <= 1) return;
            
            currentTypeIndex = (currentTypeIndex + 1 + firingLogics.Count) % firingLogics.Count;
        }

        // FIXME: Replace your init methods with factory method + single shoot() method, otherwise it's confusing for
        // those who use your API to guess they have to call both Init() on the cannon and StartFiring() for initializing ray in the logic.
        public void StartFiring()
        {
            
        }

        public void Shoot(Vector3 startPosition, Vector3 direction)
        {
            firingLogics[currentTypeIndex].Shoot(startPosition,direction);
        }

        public void FinishFiring()
        {
            firingLogics[currentTypeIndex].FinishFiring();
        }
    }
}