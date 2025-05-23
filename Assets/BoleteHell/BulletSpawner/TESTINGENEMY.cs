using UnityEngine;

public class TESTINGENEMY : MonoBehaviour
{
    [SerializeField] GameObject player;


    private BulletSpawner currentSpawner;

    private void Start()
    {
        currentSpawner = GetComponentInChildren<BulletSpawner>();
    }

    private void Update()
    {
        currentSpawner.Shoot();

    }
}
