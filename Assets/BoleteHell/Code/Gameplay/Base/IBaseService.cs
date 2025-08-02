using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Base
{
    public interface IBaseService
    {
        List<Base> Bases { get; }
       
        Base GetClosestBase(Vector2 pos, out float distance);
    }
}