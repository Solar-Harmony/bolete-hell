using UnityEngine;

namespace BoleteHell.Gameplay.SpawnManager
{
    [CreateAssetMenu(menuName = "BoleteHell/AI/Spawn List")]
    public class SpawnList : ScriptableObject
    {
        public GameObject[] allowedEnemies;
    }
}
