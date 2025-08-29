using System;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    public class ShotPattern : MonoBehaviour
    {
        private float _currentRotation;
        private float _initAttackTimer = 100f;
        private float _attackTimer = 100f;
        private float _chargeTimer;
        private Vector2 initDirection;

        private Cannon currentCannon;
        private bool canShoot;

        private void Update()
        {
            if (currentCannon == null)
                return;
            if (_attackTimer < currentCannon.cannonData.rateOfFire)
            {
                _attackTimer += Time.deltaTime;
            }
            else
            {
                canShoot = true;
            }
        }

        private void StartShooting(ShotPatternData pattern, Vector2 spawnPoint)
        {
            //Juste pour dire que le premier tir d'une arme peut être soit chargé ou instant
            //Normalement j'aurais juste fait une méthode de start et end pour setup des chose au premier tir
            //Mais l'IA de l'ennemis a l'air de call son startShooting a chaque frame faque ca marche pas trop
        
            if (Mathf.Approximately(_attackTimer, _initAttackTimer)) 
            {
                if (currentCannon.cannonData.WaitBeforeFiring)
                {
                    //TODO: ajouter un timer pour le charge time
                    _attackTimer = 0f; 
                }
            }
            
            if (canShoot)
            {
                StartCoroutine(RoutineFire(pattern, spawnPoint));
            }
        }

        private IEnumerator RoutineFire(ShotPatternData pattern, Vector2 spawnPoint)
        {
            for (int i = 0; i < pattern.burstShotCount; i++)
            {
                Fire(pattern, spawnPoint);
                yield return new WaitForSeconds(pattern.burstShotCooldown);
            }
        }

        private void Fire(ShotPatternData pattern, Vector2 spawnPoint)
        {
            float spawnDistance = Vector3.Distance(transform.position, spawnPoint);
            _currentRotation += pattern.constantRotation;
            //Angle maximal d'un coté
            int maxSideAngle = pattern.maxAngleRange / 2;
        
            for (int i = 0; i < pattern.numberOfBulletShot; i++)
            {
                SetupBulletPosition(pattern, i, maxSideAngle, spawnDistance, out Vector2 direction, out Vector2 spawnPosition);
                currentCannon.Shoot(spawnPosition, direction.normalized, this.gameObject);
            }

            _attackTimer = 0f;
            canShoot = false;
        }

        private void SetupBulletPosition(ShotPatternData pattern, int i, int maxSideAngle,
            float spawnDistance,out Vector2 direction, out Vector2 spawnPosition)
        {
            float currentAngle = SetBulletAngle(pattern.numberOfBulletShot, i, maxSideAngle);

            Quaternion rotation2D = Quaternion.Euler(0, 0, currentAngle + pattern.startingRotation + _currentRotation);
            
            direction = rotation2D * initDirection;
            spawnPosition = (Vector2)transform.position + (direction.normalized * spawnDistance);
        }

        private static float SetBulletAngle(int numberOfBullets, int i, int maxSideAngle)
        {
            float currentAngle;
            if (numberOfBullets == 1)
            {
                currentAngle = 0f; 
            }
            else
            {
                //Interpolation entre le point le plus a gauche et le point le plus a droite
                float t = (float)i / (numberOfBullets);
                currentAngle = Mathf.Lerp(-maxSideAngle, maxSideAngle, t);
            }

            return currentAngle;
        }

        public void Shoot(Cannon cannon, Vector2 spawnPosition, Vector2 initialDirection)
        {
            currentCannon = cannon;
            List<ShotPatternData> patterns = cannon.GetBulletPatterns();
            //TODO: ajouter une manière de faire que les pattern sont soit simultané ou consécutifs
            //si simultané un call de fire passe a travers tous les patterns
            //sinon un call de fire fait un pattern et le prochain call fait le pattern suivant
            //Doit fonctionner avec le patternMaster ou la liste de patterns
            initDirection = initialDirection;
            foreach (ShotPatternData pattern in patterns)
            {
                StartShooting(pattern, spawnPosition);
            }
        }
    }
}
