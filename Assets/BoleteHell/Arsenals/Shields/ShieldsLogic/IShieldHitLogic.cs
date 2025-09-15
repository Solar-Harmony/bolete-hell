using UnityEngine;

namespace BoleteHell.Arsenals.Shields.ShieldsLogic
{
    public interface IShieldHitLogic
    {
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice);
    }
}