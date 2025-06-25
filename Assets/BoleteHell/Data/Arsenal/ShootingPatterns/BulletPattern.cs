using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.RayCannon;

public class BulletPattern : MonoBehaviour
{
    private float _currentRotation;
    private float _initAttackTimer = 100f;
    private float _attackTimer = 100f;
    private Vector2 initDirection;
    private void StartShooting(Cannon cannon, BulletPatternData pattern, Transform spawnPoint)
    {
        //Juste pour dire que le premier tir d'une arme peut être soit chargé ou instant
        //Normalement j'aurais juste fait une méthode de start et end pour setup des chose au premier tir
        //Mais l'IA de l'ennemis a l'air de call son startShooting a chaque frame faque ca marche pas trop
        
        if (Mathf.Approximately(_attackTimer, _initAttackTimer)) 
        {
            if (cannon.rayCannonData.WaitBeforeFiring)
            {
                _attackTimer = 0f; 
            }
        }

        if (_attackTimer < cannon.rayCannonData.rateOfFire)
        {
            //Devrait caller une méthode charge sur le weapon pour montrer l'animation de charge du weapon i guess
            //Debug.Log($"Weapon is charging. Current charge: {_attackTimer}");
            
            _attackTimer += Time.deltaTime;
        }
        else
        {
            StartCoroutine(RoutineFire(cannon,pattern,spawnPoint));
        }
    }

    private IEnumerator RoutineFire(Cannon cannon, BulletPatternData pattern,Transform spawnPoint)
    {
        for (int i = 0; i < pattern.burstShotCount; i++)
        {
            Fire(cannon,pattern,spawnPoint);
            yield return new WaitForSeconds(pattern.burstShotCooldown);
        }
    }

    private void Fire(Cannon cannon, BulletPatternData pattern,Transform spawnPoint)
    {
        float spawnDistance = Vector3.Distance(transform.position, spawnPoint.position);
        _currentRotation += pattern.constantRotation;
        //Angle maximal d'un coté
        int maxSideAngle = pattern.maxAngleRange / 2;
        
        for (int i = 0; i < pattern.numberOfBulletShot; i++)
        {
            SetupBulletPosition(pattern, spawnPoint, i, maxSideAngle, spawnDistance,out Vector2 direction, out Vector2 spawnPosition);
            cannon.Shoot(spawnPosition, direction.normalized, this.gameObject);
        }

        _attackTimer = 0f;
    }

    private void SetupBulletPosition(BulletPatternData pattern, Transform spawnPoint, int i, int maxSideAngle,
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

    //Raycannon Pourras être changer pour une généralisation des weapons
    public void Shoot(Cannon cannon, Transform spawnPosition, Vector2 initialDirection)
    {
        List<BulletPatternData> patterns = cannon.GetBulletPatterns();
        //TODO: ajouter une manière de faire que les pattern sont soit simultané ou consécutifs
        //si simultané un call de fire passe a travers tous les patterns
        //sinon un call de fire fait un pattern et le prochain call fait le pattern suivant
        //Doit fonctionner avec le patternMaster ou la liste de patterns
        initDirection = initialDirection;
        foreach (BulletPatternData pattern in patterns)
        {
            StartShooting(cannon,pattern,spawnPosition);
        }
    }
}
