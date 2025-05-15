using System.Collections.Generic;
using Lasers;
using Prisms;
using UnityEngine;

namespace Input
{
    public class PlayerLaserInput : MonoBehaviour
    {
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private InputController input;

        private readonly List<RayCannon> _equippedPrisms = new();
        private int _selectedPrismIndex;

        private void Update()
        {
            if (input.IsShooting) Shoot();
        }

        private void OnEnable()
        {
            input.OnShootStarted += OnShootStarted;
            input.OnShootEnded += OnShootCanceled;
            input.OnCycleWeapons += CycleWeapons;
        }

        private void OnDisable()
        {
            input.OnShootStarted -= OnShootStarted;
            input.OnShootEnded -= OnShootCanceled;
            input.OnCycleWeapons -= CycleWeapons;
        }

        public void AddPrism(RayCannon rayCannon)
        {
            _equippedPrisms.Add(rayCannon);
        }

        private void CycleWeapons(int value)
        {
            if (_equippedPrisms.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedPrismIndex = (_selectedPrismIndex + value + _equippedPrisms.Count) % _equippedPrisms.Count;

            Debug.Log($"selected {GetSelectedWeapon().name}");
        }

        private RayCannon GetSelectedWeapon()
        {
            return _equippedPrisms[_selectedPrismIndex];
        }

        private void OnShootStarted()
        {
            GetSelectedWeapon().StartFiring();
        }

        private void OnShootCanceled()
        {
            GetSelectedWeapon().FinishFiring();
        }

        private void Shoot()
        {
            if (_equippedPrisms.Count == 0)
            {
                Debug.LogWarning("No prism equipped");
                return;
            }
            
            Vector2 direction = transform.up;
            GetSelectedWeapon().Shoot(bulletSpawnPoint.position, direction);
        }
    }
}