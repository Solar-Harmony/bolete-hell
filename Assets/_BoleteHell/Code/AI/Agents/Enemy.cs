using System;
using System.Collections.Generic;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using BoleteHell.RayCannon;
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

        public void Shoot(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            _weapon.Shoot( direction);
        }
    }
}