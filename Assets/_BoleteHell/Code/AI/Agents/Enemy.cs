using System;
using System.Collections.Generic;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using BoleteHell.RayCannon;
using Data.Cannons;
using Data.Rays;
using UnityEngine;

namespace AI.Agents
{
    [RequireComponent(typeof(Arsenal))]
    public class Enemy : MonoBehaviour
    {
        private Arsenal _weapon;
        
        public void Start()
        {
            _weapon =  GetComponent<Arsenal>();
        }

        public void Shoot(Vector3 direction)
        {
            _weapon.Shoot( direction);
        }
        
        // TODO: Not the best place for this but I dont know lol
        public float GetProjectileSpeed()
        {
            if (!_weapon) return 0.0f;
            return _weapon.GetSelectedWeapon().rayCannonData.projectileSpeed;
        }
    }
}