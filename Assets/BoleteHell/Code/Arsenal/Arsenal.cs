using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Gameplay.Characters;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal
{
    //Chaque RayCannonManager a son propre BulletPattern ,permet chaque a unité d'avoir son propre timer de pattern
    public class Arsenal : MonoBehaviour
    {
        [ShowInInspector, LabelText("Custom Spawn Origin")]
        [SerializeField] 
        private Transform spawnDistance;
        
        [SerializeField]
        [Min(0)]
        private float spawnRadius = 5.0f;
        
        [SerializeField]
        private List<Cannon> cannons = new();

        [Inject]
        private ICannonService _cannonService;
        
        private int _selectedCannonIndex;
        private readonly List<List<CannonInstance>> _cannonInstances = new();

        private Character _owner;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnDistance ? spawnDistance.position : transform.position, spawnRadius);
        }

        private void Start()
        {
            foreach (Cannon cannon in cannons)
            {
                var instances = new List<CannonInstance>();
                foreach (CannonConfig cannonConfig in cannon.cannonConfigs)
                {
                    var instance = new CannonInstance(cannonConfig);
                    instances.Add(instance);
                }
                _cannonInstances.Add(instances);
            }

            _owner = GetComponent<Character>();
        }
       
        private void Update()
        {
            foreach (CannonInstance weapon in GetSelectedWeapon())
            {
                _cannonService.Tick(weapon);
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
            var shotParams = new ShotLaunchParams(transform.position, spawnPosition, direction, _owner);
            

            foreach (CannonInstance weapon in GetSelectedWeapon())
            {
                _cannonService.TryShoot(weapon, shotParams);
            }
        }
        
        public float GetProjectileSpeed()
        {
            if (cannons.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return 0.0f;
            }
            
            List<CannonInstance> selectedWeapon = GetSelectedWeapon();
            
            CannonData data = selectedWeapon[0].Config.cannonData;
            return data.firingType switch
            {
                FiringTypes.Automatic => selectedWeapon[0].LaserCombo.GetLaserSpeed(),
                FiringTypes.Charged => data.cooldown,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    
        public void CycleWeapons(int value)
        {
            if (cannons.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedCannonIndex = (_selectedCannonIndex + value + cannons.Count) % cannons.Count;
        }
    
        public List<CannonInstance> GetSelectedWeapon()
        {
            if (_selectedCannonIndex < 0 || _selectedCannonIndex >= cannons.Count)
            {
                return null;
            }

            if (cannons[_selectedCannonIndex] == null || cannons[_selectedCannonIndex].cannonConfigs.Count == 0)
            {
                Debug.LogWarning("No weapons equipped");
                return null;
            }
            
            return _cannonInstances[_selectedCannonIndex];
        }

        public void SetSelectedWeapon(int index)
        {
            if (index < 0 || index >= cannonConfigs.Count)
            {
                Debug.LogWarning("Invalid weapon index");
                return;
            }

            _selectedCannonIndex = index;
        }
    
        public void OnShootCanceled()
        {
            List<CannonInstance> selectedWeapons = GetSelectedWeapon();
            if (selectedWeapons == null) 
                return;
            
            foreach (CannonInstance weapon in selectedWeapons)
            {
                _cannonService.FinishFiring(weapon);
            }
        }

        private void OnDestroy()
        {
            OnShootCanceled();
        }
    }
}
