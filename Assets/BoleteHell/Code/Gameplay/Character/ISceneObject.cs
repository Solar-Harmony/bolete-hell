using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Character
{
    public interface ISceneObject : IDamageable
    {
        Vector2 Position { get; }
    }
}