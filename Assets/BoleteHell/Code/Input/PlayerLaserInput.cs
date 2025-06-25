using UnityEngine;

namespace BoleteHell.Code.Input
{
    [RequireComponent(typeof(Arsenal.Arsenal))]
    public class PlayerLaserInput : MonoBehaviour
    {
        [SerializeField] private InputController input;
        private Arsenal.Arsenal _arsenal;
        private void Start()
        {
            _arsenal = GetComponent<Arsenal.Arsenal>();
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