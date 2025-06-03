using System;
using System.Collections.Generic;
using BoleteHell.RayCannon;
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