using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    //Made using https://thomassteffen.medium.com/burst-fire-in-unity-f2d73e025f09

    [SerializeField] GameObject projectile;
    public enum pattern 
    {
        singleBullet,
        threeSpreadBullets,
        fiveBulletArc,
    }
    public pattern bulletPattern;

    [SerializeField]
    private float timer = 100f;
    [Header("fireRate")]
    //fire rate between individual shots
    [SerializeField]
    private float _firerateCooldown;
    //fire rate between the full burst of shots
    [SerializeField]
    private float _firerateGroupCooldown;
    //adjusts the different attack frequencies
    public enum fireState
    {
        shotSingle,
        shotDouble,
        shotTriple
    }
    public fireState _fireState;
    //adjusted by the firestate
    private int _MAXshotsBeforeGroupCooldown;

    private void StatesOfFire()
    {
        switch (_fireState)
        {
            case fireState.shotSingle:
                _MAXshotsBeforeGroupCooldown = 1;
                break;
            case fireState.shotDouble:
                _MAXshotsBeforeGroupCooldown = 2;
                break;
            case fireState.shotTriple:
                _MAXshotsBeforeGroupCooldown = 3;
                break;
        }
    }

    private void CooldownTimer()
    {
        //if timer is greater than cooldown, fire
        if (timer < _firerateGroupCooldown)
        {
            timer += Time.deltaTime;
        }
        else
        {
            StartCoroutine(RoutineFire(_firerateCooldown));
        }
    }

    private IEnumerator RoutineFire(float _seconds)
    {
        for (int i = 0; i < _MAXshotsBeforeGroupCooldown; i++)
        {
            Fire();
            yield return new WaitForSeconds(_seconds);
        }
    }


    public void Fire()
    {
        switch (bulletPattern)
        {
            case pattern.singleBullet:
                //Single straight shooting bullet
                Instantiate(projectile, transform.position, transform.rotation);
                timer = 0;
                break;

            case pattern.threeSpreadBullets:
                Instantiate(projectile, transform.position, transform.rotation);
                Instantiate(projectile, transform.position, transform.rotation*Quaternion.AngleAxis(-25, Vector3.forward));
                Instantiate(projectile, transform.position, transform.rotation*Quaternion.AngleAxis(25, Vector3.forward));
                timer = 0;
                break;

            case pattern.fiveBulletArc:
                Instantiate(projectile, transform.position, transform.rotation);
                timer = 0;
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
        timer = 100f;
    }

}
