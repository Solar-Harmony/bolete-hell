using UnityEngine;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnArea : MonoBehaviour
    {
        [Tooltip("min distance in radius")]
        public float minSpawnRadius = 5f;
        
        [Tooltip("max distance in radius")]
        public float maxSpawnRadius = 50f;
        
        [Tooltip("Type of enemy list (can be organzied by biome later)")]
        public SpawnList spawnList;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
        }
    }
}

