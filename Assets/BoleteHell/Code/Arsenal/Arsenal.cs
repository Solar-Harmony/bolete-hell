using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.ShotPatterns;
using Sirenix.OdinInspector;
using UnityEngine;

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
        private List<Cannon> cannons;
        
        private int _selectedCannonIndex;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnDistance ? spawnDistance.position : transform.position, spawnRadius);
        }

        private void Start()
        {
            foreach (Cannon rayCannon in cannons)
            {
                rayCannon.Init();
            }
        }
        //TODO: faire une méthode tick dans les cannon,
        //Ajouter une méthode update ici qui call le tick du weapon courrant
        //Le tick update la valeur du timer de charge et le timer entre le tir du weapon 
        //Afin de déterminer si le joueur devrait pouvoir tirer ou pas
        
        //Je ne peut pas avoir de update dans les weapons et cette manière permet de seulement update le weapon courrant
        //Je ne peut pas avoir de bool dans le cannon data qui permettrais a d'autre script de savoir si le cannon peut tirer ou pas 
        //car le cannon data est un scriptable object et donc le modifier pour un le modifie pour tout les autres
        //Je pourrais faire la vérification dans le shotPattern mais je ne suis pas un fan,
        //le shot pattern devrait seulement gerer le tir du pattern et c'est tout

        private void Update()
        {
            GetSelectedWeapon().Tick();
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
            GetSelectedWeapon().TryShoot( spawnPosition, direction, this.gameObject);
        }
        
        public float GetProjectileSpeed()
        {
            if (cannons.Count == 0)
            {
                Debug.LogWarning("No raycannon equipped");
                return 0.0f;
            }
            
            CannonData data = GetSelectedWeapon().cannonData;
            return data.firingType switch
            {
                FiringTypes.Automatic => data.projectileSpeed,
                FiringTypes.Charged => data.rateOfFire,
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
