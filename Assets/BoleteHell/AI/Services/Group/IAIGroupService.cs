using UnityEngine;

namespace BoleteHell.AI.Services.Group
{
    public interface IAIGroupService
    {
        AIGroup CreateGroup();
        void AddToGroup(int groupID, GameObject obj);
        AIGroup GetGroup(int groupID);
    }
}