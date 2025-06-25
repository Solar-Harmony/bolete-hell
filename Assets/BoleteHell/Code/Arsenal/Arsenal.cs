using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoleteHell.RayCannon
{
    //Chaque RayCannonManager a son propre BulletPattern ,permet chaque a unité d'avoir son propre timer de pattern
    [RequireComponent(typeof(ShotPattern))]
    public class Arsenal : MonoBehaviour
    {
        [SerializeField] private Transform spawnDistance;
        [SerializeReference] private List<Cannon> rayCannons;
        private ShotPattern _pattern;
        private int _selectedCannonIndex;

        private void Start()
        {
            _pattern = GetComponent<ShotPattern>();
            foreach (Cannon rayCannon in rayCannons)
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

            _pattern.Shoot(GetSelectedWeapon(), spawnDistance, direction);
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
    
        public Cannon GetSelectedWeapon()
        {
            if (rayCannons[_selectedCannonIndex] != null) return rayCannons[_selectedCannonIndex];
            
            Debug.LogWarning("No weapons equipped");
            return null;
        }
    
        public void OnShootCanceled()
        {
            GetSelectedWeapon()?.FinishFiring();
        }

        public void AddNewWeapon(Cannon cannon)
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
