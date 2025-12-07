using UnityEngine;

namespace BoleteHell.Gameplay.Droppables
{
    public interface IDropManager
    {
        public void TryDropLoot(GameObject dropSource, LootTable ctx);
    }
}
