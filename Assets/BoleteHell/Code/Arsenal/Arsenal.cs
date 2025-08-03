using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.ShotPatterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal
{
    //Chaque RayCannonManager a son propre BulletPattern ,permet chaque a unité d'avoir son propre timer de pattern
    [RequireComponent(typeof(ShotPattern))]
    public class Arsenal : MonoBehaviour
    {
        [ShowInInspector, LabelText("Custom Spawn Origin")]
        [SerializeField] 
        private Transform spawnDistance;
        
        [SerializeField]
        [Min(0)]
        private float spawnRadius = 5.0f;
        
        [SerializeReference] private List<Cannon> cannons;
        private ShotPattern _pattern;
        private int _selectedCannonIndex;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnDistance ? spawnDistance.position : transform.position, spawnRadius);
        }

        private void Start()
        {
            _pattern = GetComponent<ShotPattern>();
            foreach (Cannon rayCannon in cannons)
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

            // TODO: We could just use the circle collider radius but then wouldn't work with non-circular colliders
            Vector2 spawnOrigin = spawnDistance ? spawnDistance.position : transform.position;
            Vector2 spawnPosition = spawnOrigin + direction * spawnRadius;
            _pattern.Shoot(GetSelectedWeapon(), spawnPosition, direction);
        }
    
        public void CycleWeapons(int value)
        {
            if (cannons.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedCannonIndex = (_selectedCannonIndex + value + cannons.Count) % cannons.Count;

            Debug.Log($"selected {GetSelectedWeapon().cannonData.name}");
        }
    
        public Cannon GetSelectedWeapon()
        {
            if (_selectedCannonIndex < 0 || _selectedCannonIndex >= cannons.Count)
            {
                return null;
            }

            if (cannons[_selectedCannonIndex] == null)
            {
                Debug.LogWarning("No weapons equipped");
                return null;
            }
            
            return cannons[_selectedCannonIndex];
        }
    
        public void OnShootCanceled()
        {
            GetSelectedWeapon()?.FinishFiring();
        }

        public void AddNewWeapon(Cannon cannon)
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
