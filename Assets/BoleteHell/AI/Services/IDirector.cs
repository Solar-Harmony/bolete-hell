using UnityEngine;

namespace BoleteHell.AI.Services
{
    public interface IDirector
    {
        GameObject FindWeakestAlly(GameObject self);
        GameObject FindNearestAlly(GameObject self);
        GameObject FindNearestTarget(GameObject self);
    }
}