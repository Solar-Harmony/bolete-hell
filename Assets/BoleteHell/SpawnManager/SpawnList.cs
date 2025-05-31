using UnityEngine;

[CreateAssetMenu(menuName = "AI/SpawnList")]
public class SpawnList : ScriptableObject
{
    public EnemyData[] allowedEnemies;
}
