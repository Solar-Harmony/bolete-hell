
using UnityEngine;

namespace BoleteHell.Gameplay.SpawnManager
{
    public record SpawnParams(GameObject prefab, Vector2 position, int groupID);
    
    public interface ISpawnService
    {
        // Try to spawn an enemy at the nearest spawn area. 
        public bool SpawnInArea(SpawnParams parameters);
        
        // Try to spawn an enemy at the exact given position.
        public bool SpawnAt(SpawnParams parameters);
    }
}