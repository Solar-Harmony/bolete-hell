
using UnityEngine;

namespace BoleteHell.Gameplay.SpawnManager
{
    public interface ISpawnService
    {
        /// <summary>
        /// Spawn an entity around the chosen spawn area.
        /// </summary>
        /// <param name="spawnArea">A spawn area object, placed in the world.</param>
        /// <returns>Whether spawning succeeded or failed.</returns>
        public bool Spawn(SpawnArea spawnArea);

        /// <summary>
        /// Spawn all entities in a spawn list at a given position.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Spawn(SpawnList list, Vector2 position, int groupID = -1);
    }
}