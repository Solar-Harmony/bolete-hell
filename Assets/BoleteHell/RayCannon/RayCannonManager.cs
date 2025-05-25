using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.RayCannon
{
    public class RayCannonManager : MonoBehaviour
    {
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private List<RayCannon> rayCannonsData;
        private int _selectedCannonIndex;

        private void Start()
        {
            foreach (RayCannon rayCannon in rayCannonsData)
            {
                rayCannon.Init();
            }
        }

        public void Shoot(Vector2 direction)
        {
            if (rayCannonsData.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return;
            }
            
            GetSelectedWeapon().Shoot(bulletSpawnPoint.position, direction);
        }
    
        public void CycleWeapons(int value)
        {
            if (rayCannonsData.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedCannonIndex = (_selectedCannonIndex + value + rayCannonsData.Count) % rayCannonsData.Count;

            Debug.Log($"selected {GetSelectedWeapon()}");
        }
    
        private RayCannon GetSelectedWeapon()
        {
            return rayCannonsData[_selectedCannonIndex];
        }
    
        public void OnShootCanceled()
        {
            GetSelectedWeapon().FinishFiring();
        }

        private void OnDestroy()
        {
            OnShootCanceled();
        }
    }
}
