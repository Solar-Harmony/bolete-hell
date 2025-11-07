using UnityEngine;

namespace BoleteHell.Code.Gameplay.Droppables
{
    public interface IDropManager
    {
        public void Drop(GameObject dropSource, DropSettings ctx);
        public void DropDroplets(GameObject dropSource, DropRangeContext ctx);
        public void DropGold(GameObject dropSource, DropRangeContext ctx);
    }
}
