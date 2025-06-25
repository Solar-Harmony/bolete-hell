using System.Collections.Generic;
using BoleteHell.Code.Arsenal.ShotPatterns;
using UnityEngine;

namespace BoleteHell.Code.Arsenal
{
    //Chaque RayCannonManager a son propre BulletPattern ,permet chaque a unité d'avoir son propre timer de pattern
    [RequireComponent(typeof(ShotPattern))]
    public class Arsenal : MonoBehaviour
    {
        [SerializeField] private Transform spawnDistance;
        [SerializeReference] private List<Cannons.Cannon> cannons;
        private ShotPattern _pattern;
        private int _selectedCannonIndex;

        private void Start()
        {
            _pattern = GetComponent<ShotPattern>();
            foreach (Cannons.Cannon rayCannon in cannons)
            {
                rayCannon.Init();
            }
        }

        public void Shoot(Vector2 direction)
        {
            if (cannons.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return;
            }

            _pattern.Shoot(GetSelectedWeapon(), spawnDistance, direction);
        }
    
        public void CycleWeapons(int value)
        {
            if (cannons.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedCannonIndex = (_selectedCannonIndex + value + cannons.Count) % cannons.Count;

            Debug.Log($"selected {GetSelectedWeapon()}");
        }
    
        public Cannons.Cannon GetSelectedWeapon()
        {
            if (cannons[_selectedCannonIndex] != null) return cannons[_selectedCannonIndex];
            
            Debug.LogWarning("No weapons equipped");
            return null;
        }
    
        public void OnShootCanceled()
        {
            GetSelectedWeapon()?.FinishFiring();
        }

        public void AddNewWeapon(Cannons.Cannon cannon)
        {
            cannon.Init();
            cannons.Add(cannon);
        }

        private void OnDestroy()
        {
            OnShootCanceled();
        }
    }
}
