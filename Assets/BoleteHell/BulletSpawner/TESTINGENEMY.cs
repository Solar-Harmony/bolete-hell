using UnityEngine;

public class TESTINGENEMY : MonoBehaviour
{
    [SerializeField] GameObject player;


    private BulletPattern _currentPattern;

    private void Start()
    {
        _currentPattern = GetComponentInChildren<BulletPattern>();
    }

    private void Update()
    {
        //_currentPattern.Shoot();

    }
}
