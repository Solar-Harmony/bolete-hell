using System;
using System.Collections.Generic;
using BoleteHell.RayCannon;
using Lasers;
using UnityEngine;

namespace Input
{
    [RequireComponent(typeof(RayCannonManager))]
    public class PlayerLaserInput : MonoBehaviour
    {
        [SerializeField] private InputController input;
        private RayCannonManager rayCannonManager;
        private void Start()
        {
            rayCannonManager = GetComponent<RayCannonManager>();
        }

        private void Update()
        {
            if (input.IsShooting) Shoot();
        }

        private void OnEnable()
        {
            input.OnShootEnded += OnShootCanceled;
            input.OnCycleWeapons += CycleWeapons;
        }

        private void OnDisable()
        {
            input.OnShootEnded -= OnShootCanceled;
            input.OnCycleWeapons -= CycleWeapons;
        }

        private void CycleWeapons(int value)
        {
            rayCannonManager.CycleWeapons(value);
        }

        private void OnShootCanceled()
        {
            rayCannonManager.OnShootCanceled();
        }

        private void Shoot()
        {
            rayCannonManager.Shoot(transform.up);
        }
    }
}