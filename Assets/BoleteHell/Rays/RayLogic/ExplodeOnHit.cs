using System;
using UnityEngine;

namespace Lasers.RayLogic
{
    [Serializable]
    public class ExplodeOnHit : RayHitLogic
    {
        [SerializeField] private float explosionWidth;

        public override void OnHit()
        {
            Debug.Log("Boom");
        }
    }
}