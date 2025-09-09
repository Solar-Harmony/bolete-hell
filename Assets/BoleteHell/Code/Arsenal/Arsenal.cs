using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
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
        
        [SerializeReference] [HideReferenceObjectPicker] 
        private List<CannonConfig> cannonConfigs;

        [Inject]
        private ICannonService _cannonService;
        
        private int _selectedCannonIndex;
        private readonly List<CannonInstance> _cannons = new();
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnDistance ? spawnDistance.position : transform.position, spawnRadius);
        }

        private void Start()
        {
            foreach (CannonConfig cannonConfig in cannonConfigs)
            {
                // TODO: we can simplify this
                var instance = new CannonInstance(cannonConfig);
                _cannons.Add(instance);
            }
        }
       
        private void Update()
        {
            _cannonService.Tick(GetSelectedWeapon());
        }

        public void Shoot(Vector2 direction)
        {
            if (cannonConfigs.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return;
            }

            // TODO: We could just use the circle collider radius but then wouldn't work with non-circular colliders
            Vector2 spawnOrigin = spawnDistance ? spawnDistance.position : transform.position;
            Vector2 spawnPosition = spawnOrigin + direction * spawnRadius;
            var shotParams = new ShotParams(spawnPosition, direction, this.gameObject);
            
            _cannonService.TryShoot(GetSelectedWeapon(), shotParams);
        }
        
        public float GetProjectileSpeed()
        {
            if (cannonConfigs.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return 0.0f;
            }
            
            CannonData data = GetSelectedWeapon().Config.cannonData;
            return data.firingType switch
            {
                FiringTypes.Automatic => data.projectileSpeed,
                FiringTypes.Charged => data.rateOfFire,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    
        public void CycleWeapons(int value)
        {
            if (cannonConfigs.Count <= 1)
            {
                Debug.LogWarning("No weapons to cycle trough");
                return;
            }

            _selectedCannonIndex = (_selectedCannonIndex + value + cannonConfigs.Count) % cannonConfigs.Count;

            Debug.Log($"selected {GetSelectedWeapon().Config.cannonData.name}");
        }
    
        public CannonInstance GetSelectedWeapon()
        {
            if (_selectedCannonIndex < 0 || _selectedCannonIndex >= cannonConfigs.Count)
            {
                return null;
            }

            if (cannonConfigs[_selectedCannonIndex] == null)
            {
                Debug.LogWarning("No weapons equipped");
                return null;
            }
            
            return _cannons[_selectedCannonIndex];
        }
    
        public void OnShootCanceled()
        {
            CannonInstance selectedWeapon = GetSelectedWeapon();
            if (selectedWeapon == null) 
                return;
            
            _cannonService.FinishFiring(selectedWeapon);
        }

        private void OnDestroy()
        {
            OnShootCanceled();
        }
    }
}
