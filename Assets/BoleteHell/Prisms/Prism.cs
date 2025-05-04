using UnityEngine;

public class Prism : MonoBehaviour
{
    [Tooltip("bool for prototyping purposes True = automatic, False = charged")]
    [SerializeField] private bool firingType;

    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float chargingSpeed;
    [SerializeField] private float reloadTime;
    [SerializeField] private int numBulletsPerShot;
    [SerializeField] private float spreadAngle;
    [SerializeField] private float precision;
    //TODO: Add logic to be able to combine rays (not spawn two rays at the same time really make 1 ray have the effects of two different rays (buffs included))
    //maybe combining rays adds logic to the first prisms ray
    //so when you combine two rays you choose one prism in which two rays's logic is combined
    //Or we actually combine the two prisms and average the stats and add the ray logic to the ray
    //Should check if we want to be able to de-combine the prisms after too
    [SerializeField] private Ray ray;

    public void Shoot(Vector3 startPosition, Vector3 direction)
    {
        ray.Cast(startPosition,direction);
    }
}
