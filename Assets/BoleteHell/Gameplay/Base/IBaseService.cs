using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Gameplay.Base
{
    public interface IBaseService
    {
        List<Base> Bases { get; }

        void NotifyBaseDied(Base theBase);
        Base GetClosestBase(Vector2 pos, out float distance);
    }
}