using BoleteHell.Arsenals.Rays;
using BoleteHell.Code.Audio.BoleteHell.Models;
using UnityEngine;

namespace BoleteHell.Arsenals.LaserData
{
    public interface IOnHitLogic
    {
        public void OnHit(Vector2 hitPosition, IDamageable hitObject, LaserInstance laserInstance, LaserData data);
    }
}