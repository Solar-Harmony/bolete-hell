using System;
using System.Collections.Generic;
using Prisms;
using UnityEngine;

namespace AI.Agents
{
    [RequireComponent(typeof(RayCannon))]
    public class Enemy : MonoBehaviour
    {
        private RayCannon _weapon;
        
        public void Start()
        {
            _weapon =  GetComponent<RayCannon>();
            _weapon.Init();
        }

        public void Shoot(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            _weapon.StartFiring();
            _weapon.Shoot(transform.position, direction);
        }
    }
}