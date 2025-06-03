using System;
using System.Collections.Generic;
using BoleteHell.RayCannon;
using Lasers;
using UnityEngine;

namespace Input
{
    [RequireComponent(typeof(Arsenal))]
    public class PlayerLaserInput : MonoBehaviour
    {
        [SerializeField] private InputController input;
        private Arsenal _arsenal;
        private void Start()
        {
            _arsenal = GetComponent<Arsenal>();
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
            _arsenal.CycleWeapons(value);
        }

        private void OnShootCanceled()
        {
            _arsenal.OnShootCanceled();
        }

        private void Shoot()
        {
            _arsenal.Shoot(transform.up);
        }
    }
}