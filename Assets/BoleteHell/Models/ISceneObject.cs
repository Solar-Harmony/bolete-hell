using UnityEngine;

namespace BoleteHell.Code.Audio.BoleteHell.Models
{
    public interface ISceneObject : IDamageable
    {
        Vector2 Position { get; }
    }
}