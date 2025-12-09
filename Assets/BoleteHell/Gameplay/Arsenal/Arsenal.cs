using System;
using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Gameplay.Characters;
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

        [Inject] private ICannonService _cannonService;
        
        public event Action<Cannon> OnWeaponChanged;
        
        private int _selectedCannonIndex;
        private readonly List<List<CannonInstance>> _cannonInstances = new();
        private GameObject _owner;
        private EnergyComponent _energyComponent;
        
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

            _owner = gameObject;
            _owner.GetComponent<HealthComponent>().OnDeath += OnShootCanceled;
            _owner.TryGetComponent(out _energyComponent);
        }
       
        private void Update()
        {
            foreach (CannonInstance weapon in GetSelectedWeapons())
            {
                _cannonService.Tick(weapon);
            }
        }

        public bool IsReadyToShoot()
        {
            return GetSelectedWeapons().Any(weapon => weapon.CanShoot) && HasEnergyToFire();
        }

        private bool HasEnergyToFire()
        {
            bool canFire = true;
            
            if (_energyComponent)
            {
                float totalEnergyCost = 0f;
                
                foreach (CannonInstance selectedWeapon in GetSelectedWeapons())
                {
                    totalEnergyCost += selectedWeapon.Config.cannonData.energyCostFire;
                }

                canFire = _energyComponent.CanSpend(totalEnergyCost);
            }

            return canFire;
        }

        public bool Shoot(Vector2 direction)
        {
            if (cannons.Count == 0)
            {
                Debug.LogWarning("Cannot shoot: no cannon in arsenal.");
                return false;
            }
            
            Vector2 spawnOrigin = spawnDistance ? spawnDistance.position : transform.position;
            Vector2 spawnPosition = spawnOrigin + direction * spawnRadius;
            var shotParams = new ShotLaunchParams(transform.position, spawnPosition, direction, _owner);
            
            bool doneFiring = true;

            foreach (CannonInstance weapon in GetSelectedWeapons())
            {
                if (!_cannonService.TryShoot(weapon, shotParams))
                {
                    doneFiring = false;
                }
                else
                {
                    if (_energyComponent)
                    {
                        _energyComponent.Spend(weapon.Config.cannonData.energyCostFire);
                    }
                }
            }
            
            return doneFiring;
        }
        
        public float GetProjectileSpeed()
        {
            if (cannons.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return 0.0f;
            }
            
            List<CannonInstance> selectedWeapon = GetSelectedWeapons();
            
            CannonData data = selectedWeapon[0].Config.cannonData;
            return data.firingType switch
            {
                FiringTypes.Automatic => selectedWeapon[0].LaserCombo.GetLaserSpeed(),
                FiringTypes.Charged => data.GetCooldown(selectedWeapon[0].ShotCount),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    
        public void CycleWeapons(int value)
        {
            if (cannons.Count <= 1)
            {
                return;
            }

            SetSelectedWeaponIndex((_selectedCannonIndex + value + cannons.Count) % cannons.Count);
        }
    
        public List<CannonInstance> GetSelectedWeapons()
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

        public void SetSelectedWeaponIndex(int index)
        {
            if (index < 0 || index >= _cannonInstances.Count)
            {
                Debug.LogWarning("Invalid weapon index");
                return;
            }
            OnShootCanceled();
            _selectedCannonIndex = index;
            
            OnWeaponChanged?.Invoke(cannons[_selectedCannonIndex]);
        }
    
        public void OnShootCanceled()
        {
            List<CannonInstance> selectedWeapons = GetSelectedWeapons();
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
        
        //Je n'accepte pas de déscendre en bas de 1 car quand l'éffet de slowFireRate va être retirer
        //il est possible que l'unité affecté ai changer de weapon équipper donc celui la aurait sont cooldown réduit
        //Plutot que l'arme qui avait le malus
        public void UpdateCooldownModifier(float value)
        {
            foreach (CannonInstance selectedWeapon in GetSelectedWeapons())
            {
                float updatedValue = Mathf.Clamp(selectedWeapon.CooldownModifier + value, 1, Mathf.Infinity); 
                selectedWeapon.CooldownModifier = updatedValue;
            }
        }

        public void SetCooldownModifier(float value)
        {
            foreach (CannonInstance selectedWeapon in GetSelectedWeapons())
            {
                selectedWeapon.CooldownModifier = value;
            }
            OnShootCanceled();
        }
    }
}
