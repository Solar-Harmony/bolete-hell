using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoleteHell.RayCannon
{
    //Chaque RayCannonManager a son propre BulletPattern ,permet chaque a unité d'avoir son propre timer de pattern
    [RequireComponent(typeof(BulletPattern))]
    public class RayCannonManager : MonoBehaviour
    {
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeReference] private List<RayCannon> rayCannons;
        private BulletPattern _pattern;
        private int _selectedCannonIndex;

        private void Start()
        {
            _pattern = GetComponent<BulletPattern>();
            foreach (RayCannon rayCannon in rayCannons)
            {
                rayCannon.Init();
            }
        }

        public void Shoot(Vector2 direction)
        {
            if (rayCannons.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return;
            }
            _pattern.Shoot(GetSelectedWeapon(),bulletSpawnPoint);
            
        }
    
        public void CycleWeapons(int value)
        {
            if (rayCannons.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedCannonIndex = (_selectedCannonIndex + value + rayCannons.Count) % rayCannons.Count;

            Debug.Log($"selected {GetSelectedWeapon()}");
        }
    
        public RayCannon GetSelectedWeapon()
        {
            if (rayCannons[_selectedCannonIndex] != null) return rayCannons[_selectedCannonIndex];
            
            Debug.LogWarning("No weapons equipped");
            return null;
        }
    
        public void OnShootCanceled()
        {
            GetSelectedWeapon()?.FinishFiring();
        }

        public void AddNewWeapon(RayCannon cannon)
        {
            cannon.Init();
            rayCannons.Add(cannon);
        }

        private void OnDestroy()
        {
            OnShootCanceled();
        }
    }
}
