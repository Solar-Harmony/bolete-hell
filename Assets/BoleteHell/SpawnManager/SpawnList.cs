using UnityEngine;

[CreateAssetMenu(menuName = "AI/SpawnList")]
public class SpawnList : ScriptableObject
{
    public EnemyData[] allowedEnemies;
    public EnemyData eliteEnemy;

}
// PLs make an asset folder for this and organize them by biome