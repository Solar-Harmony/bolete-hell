using BoleteHell.Code.Arsenal;
using BoleteHell.Code.Input;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.InputControllers
{
    [RequireComponent(typeof(Arsenal))]
    public class ShotInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        private Arsenal _arsenal;
        private void Start()
        {
            _arsenal = GetComponent<Arsenal>();
        }

        private void Update()
        {
            if (input.IsChargingShot) Shoot();
        }

        private void OnEnable()
        {
            input.OnShoot += OnShootCanceled;
            input.OnCycleWeapons += CycleWeapons;
        }

        private void OnDisable()
        {
            input.OnShoot -= OnShootCanceled;
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