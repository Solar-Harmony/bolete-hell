using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BoleteHell.Gameplay.Characters.Registry
{
    public enum EntityTag
    {
        Player,
        Enemy,
        EliteEnemy,
        Base
    }
    
    public enum SuperlativeQuery
    {
        Lowest,
        Highest
    }
    
    public interface IEntityRegistry
    {
        GameObject GetPlayer();
        IEnumerable<GameObject> WithTag(EntityTag tag);
        int GetCount(EntityTag tag);
        [CanBeNull] GameObject GetWeakestEliteAlive();
        
        // Returns the base with the lowest distance to the given position, or null if there are no bases.
        GameObject GetClosestBase(Vector2 pos, out float distance);
        
        // Returns the base with the lowest health, or null if there are no bases.
        GameObject GetWeakestBase();
        
        event Action<(EntityTag[], GameObject)> EntityDied;
        
        void Register(EntityTag[] tags, GameObject entity);
        void Unregister(EntityTag[] tags, GameObject entity);
    }
}