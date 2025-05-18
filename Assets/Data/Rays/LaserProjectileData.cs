using Lasers;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserProjectileData", menuName = "Scriptable Objects/Laser/LaserProjectileData")]
public class LaserProjectileData : LaserData
{
    [field: SerializeField] public float laserLenght { get; private set; } = 0.3f;
}
