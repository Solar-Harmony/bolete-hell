using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Base
{
    public interface IBaseService
    {
        List<Base> Bases { get; }

        void NotifyBaseDied(Base theBase);
        
        // Returns the base with the lowest distance to the given position, or null if there are no bases.
        Base GetClosestBase(Vector2 pos, out float distance);
        
        // Returns the base with the lowest health, or null if there are no bases.
        Base GetWeakestBase();
    }
}