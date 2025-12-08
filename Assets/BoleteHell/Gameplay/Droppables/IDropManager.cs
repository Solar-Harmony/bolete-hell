using UnityEngine;

namespace BoleteHell.Gameplay.Droppables
{
    public interface IDropManager
    {
        public void Drop(GameObject dropSource, GameObject droplet, LootTable ctx);
    }
}
