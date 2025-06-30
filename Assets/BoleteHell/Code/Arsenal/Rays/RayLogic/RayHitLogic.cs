using System;
using BoleteHell.Code.Character;
using BoleteHell.Code.Gameplay.Health;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        [SerializeField] protected int baseHitDamage;
        //Au lieu de health on pourrais avoir un component de stats en général comme ça les tir pourrait affecter le stat qu'il veut directement
        //(réduire le mouvement, reduire l'attaque wtv)
        public abstract void OnHit(Vector2 hitPosition, IHealth hitCharacterHealth);
    }
}