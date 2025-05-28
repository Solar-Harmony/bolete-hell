using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Tooltip("Normal Enemy Prefab")]
    public GameObject normalPrefab;

    //[Tooltip("Elite Enemy Prefab")]
    //public GameObject elitePrefab;

}
