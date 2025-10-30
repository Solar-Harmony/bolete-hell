using UnityEngine;

namespace BoleteHell.Code.Gameplay.SpawnManager
{
    [CreateAssetMenu(menuName = "BoleteHell/AI/Spawn List")]
    public class SpawnList : ScriptableObject
    {
        public GameObject[] allowedEnemies;
    }
}
