using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Base
{
    [RequireComponent(typeof(Renderer))]
    public class Base : MonoBehaviour, ITargetable, ISceneObject
    {
        public Vector2 Position => transform.position;
        public Health Health { get; } = new();

        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            
        }
    }
}