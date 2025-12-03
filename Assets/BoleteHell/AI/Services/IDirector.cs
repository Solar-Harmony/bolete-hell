using UnityEngine;

namespace BoleteHell.AI.Services
{
    public interface IDirector
    {
        GameObject FindWeakestAlly(GameObject self);
        GameObject FindTarget(GameObject self, int groupID);
    }
}