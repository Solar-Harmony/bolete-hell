using System.Collections.Generic;
using Lasers;
using UnityEngine;
using Ray = Lasers.Ray;

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
        //Peut-être pas besoin du dictionnary
        [SerializeReference] private List<RayCannonFiringLogic> firingLogics;
        
        private int currentTypeIndex = 0;


        //TODO: Add logic to be able to combine rays (not spawn two rays at the same time really make 1 ray have the effects of two different rays (buffs included))
        //maybe combining rays adds logic to the first prisms ray
        //so when you combine two rays you choose one prism in which two rays's logic is combined
        //Or we actually combine the two prisms and average the stats and add the ray logic to the ray
        //Should check if we want to be able to de-combine the prisms after too
        [SerializeField] private Ray ray;
        private Ray _modifiableRay;

        public void SwitchFiringType()
        {
            if (firingLogics.Count > 1) return;
            
            currentTypeIndex = (currentTypeIndex + 1 + firingLogics.Count) % firingLogics.Count;
        }

        public void Init()
        {
            _modifiableRay = Instantiate(ray);
            
        }

        public void StartFiring()
        {
            firingLogics[currentTypeIndex].StartFiring(_modifiableRay);
        }

        public void Shoot(Vector3 startPosition, Vector3 direction)
        {
            firingLogics[currentTypeIndex].Shoot(startPosition,direction);
            //_modifiableRay.Cast(startPosition, direction, lineRenderer);
        }

        public void FinishFiring()
        {
            firingLogics[currentTypeIndex].FinishFiring();
        }


    }
}