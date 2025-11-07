using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields.ShieldsLogic
{
    public interface IShieldHitLogic
    {
        public bool ShouldBlocklaser { get; set; }
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice);
    }
}