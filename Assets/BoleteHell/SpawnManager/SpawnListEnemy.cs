using UnityEngine;

[CreateAssetMenu(menuName = "AI/SpawnList")]
public class SpawnListEnemy : ScriptableObject
{
    public GameObject[] allowedEnemies;
}
