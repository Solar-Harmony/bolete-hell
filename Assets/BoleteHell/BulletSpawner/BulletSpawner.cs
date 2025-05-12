using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    //Made using https://thomassteffen.medium.com/burst-fire-in-unity-f2d73e025f09

    [SerializeField] GameObject projectile;

    [SerializeField]
    private float attackTimer = 100f;
    public enum pattern
    {
        singleBullet,
        threeSpreadBullets,
        fiveBulletArc,
        bulletHell,
    }
    public pattern bulletPattern;

    //fire rate between individual shots
    [SerializeField]
    private float _firerateCooldown;
    //fire rate between the full burst of shots
    [SerializeField]
    private float _AttackCooldown;
    //adjusts the different attack frequencies
    public enum fireState
    {
        shotSingle,
        shotDouble,
        shotTriple,
        shotConstant
    }
    public fireState _fireState;
    //adjusted by the firestate
    private int _MAXshotsBeforeAttackCooldown;

    private void StatesOfFire()
    {
        switch (_fireState)
        {
            case fireState.shotSingle:
                _MAXshotsBeforeAttackCooldown = 1;
                break;
            case fireState.shotDouble:
                _MAXshotsBeforeAttackCooldown = 2;
                break;
            case fireState.shotTriple:
                _MAXshotsBeforeAttackCooldown = 3;
                break;
            case fireState.shotConstant:
                _MAXshotsBeforeAttackCooldown = 1048;
                break;
        }
    }

    private void CooldownTimer()
    {
        //if timer is greater than cooldown, fire
        if (attackTimer < _AttackCooldown)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            StartCoroutine(RoutineFire(_firerateCooldown));
        }
    }

    private IEnumerator RoutineFire(float _seconds)
    {
        for (int i = 0; i < _MAXshotsBeforeAttackCooldown; i++)
        {
            Fire(i);
            yield return new WaitForSeconds(_seconds);
        }
    }

    // burstOrder allows the patterns to change depending on their order in a given burst, IE bulletHell crisscrossing its pattern
    public void Fire(int burstOrder)
    {
        switch (bulletPattern)
        {
            //Single straight shooting bullet
            case pattern.singleBullet:
                Instantiate(projectile, transform.position, transform.rotation);
                attackTimer = 0;
                break;

            //Shotgun Spread
            case pattern.threeSpreadBullets:
                Instantiate(projectile, transform.position, transform.rotation);
                Instantiate(projectile, transform.position, transform.rotation*Quaternion.AngleAxis(-25, Vector3.forward));
                Instantiate(projectile, transform.position, transform.rotation*Quaternion.AngleAxis(25, Vector3.forward));
                attackTimer = 0;
                break;

            case pattern.fiveBulletArc:
                Instantiate(projectile, transform.position, transform.rotation);
                attackTimer = 0;
                break;

            case pattern.bulletHell:
                int radius = 0 + ((burstOrder - 1) * 15);
                for (int i = 0; i <= 12; i++)
                {
                    Instantiate(projectile, transform.position, transform.rotation * Quaternion.AngleAxis(radius, Vector3.forward));
                    radius += 30;
                }
                attackTimer = 0;
                break;

        }
    }

    public void Shoot()
    {
        StatesOfFire();
        CooldownTimer();

    }

    public void Stop()
    {
        //Setting it to 100 allows the moment it is called to fire upon seeing the player to instantly issue the fire command.
        attackTimer = 100f;
    }

}
