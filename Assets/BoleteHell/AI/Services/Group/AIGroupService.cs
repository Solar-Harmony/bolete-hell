using System.Collections.Generic;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Enemy;
using UnityEngine;

namespace BoleteHell.AI.Services.Group
{
    public class AIGroupService : IAIGroupService
    {
        private int _nextGroupID = 0;
        
        private readonly Dictionary<int, AIGroup> _groups = new();
        
        public int CreateGroup()
        {
            int groupID = _nextGroupID;
            
            AIGroup newGroup = new AIGroup
            {
                GroupID = groupID
            };
            _groups.Add(groupID, newGroup);
            
            _nextGroupID++;
            return groupID;
        }

        public void AddToGroup(int groupID, GameObject obj)
        {
            AIGroupComponent groupComponent = obj.GetComponent<AIGroupComponent>();
            Debug.Assert(groupComponent, "Object must have an AIGroupComponent to be added to a group.");
            
            groupComponent.GroupID = groupID;
            
            AIGroup group = _groups[groupID];
            group.NumMembers++;
            
            HealthComponent healthComponent = obj.GetComponent<HealthComponent>();
            Debug.Assert(healthComponent, "Object must have a HealthComponent to be added to a group.");
            healthComponent.OnDeath += () =>
            {
                group.NumMembers--;
                if (group.NumMembers <= 0)
                {
                    _groups.Remove(groupID);
                }
            };
        }

        public AIGroup GetGroup(int groupID)
        {
            return _groups[groupID];
        }
    }
}