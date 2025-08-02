using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Input.Controllers
{
    [RequireComponent(typeof(Arsenal.Arsenal))]
    public class ShotInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        private Arsenal.Arsenal _arsenal;
        private void Start()
        {
            _arsenal = GetComponent<Arsenal.Arsenal>();
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