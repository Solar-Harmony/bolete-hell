using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Tooltip("Normal Enemy Prefab")]
    public GameObject normalPrefab;

    [Tooltip("Elite Enemy Prefab")]
    public GameObject elitePrefab;

    [Header("Spawn Radius (units from player)")]
    [Tooltip("Minimum Radius")]
    public float minSpawnRadius;

    [Tooltip("Maximum Radius")]
    public float maxSpawnRadius;

    [Header("Spawn Weight")]
    [Tooltip("The $$$ The spawn manager has to use")] //customize everything for each unit pls lol
    public int spawnWeight = 1;
}
//Hello, if youre watching this you want to go to Create/Ai/EnemyData and create an asset with all the data of the enemies mentioned above. then drag the asset to the asset you will create in SpawnList.cs