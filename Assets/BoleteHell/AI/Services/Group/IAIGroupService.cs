using UnityEngine;

namespace BoleteHell.AI.Services.Group
{
    public interface IAIGroupService
    {
        int CreateGroup();
        void AddToGroup(int groupID, GameObject obj);
        AIGroup GetGroup(int groupID);
    }
}