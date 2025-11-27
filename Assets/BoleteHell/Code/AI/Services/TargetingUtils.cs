using BoleteHell.Code.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.AI.Services
{
    public class TargetingUtils : ITargetingUtils
    {
        //private string playerLayer = "Unit";
        private string enemyLayer = "PlayerEnemy";
        private float radius = 0.17f;
        public bool HasLineOfSight(GameObject self, GameObject target, float viewRange)
        {
            if (!self || !target)
                return false;
        
            Vector3 direction = target.transform.position - self.transform.position;
            
            //Permet a l'ennemis de ne pas prendre les autre ennemis en compte pour target le joueur ou les bases
            //mais de prendre les autre ennemis en compte quand il target un ennemis
            var faction = self.GetComponent<FactionComponent>();
            string ignoreLayer = faction.Type == FactionType.Enemy ? "" : enemyLayer;
            LayerMask layerMask = ~LayerMask.GetMask(ignoreLayer, "IgnoreProjectile", "Shield");
            
            //j'avance le cast un peut pour ne pas se hit soit même
            RaycastHit2D hit = Physics2D.CircleCast(self.transform.position + direction.normalized * 0.8f, radius, direction.normalized, viewRange, layerMask);
            Debug.DrawRay(self.transform.position + direction.normalized * 0.8f, direction.normalized * viewRange);
            return hit.collider && hit.collider.gameObject == target.gameObject;
        }

        /// <summary>
        /// Solves the direction needed to hit a moving target with a projectile.
        /// Note that if the target moves too fast for the given projectile speed, there will be no solution.
        /// </summary>
        /// <param name="projectileDir"></param>
        /// <param name="projectileSpeed"></param>
        /// <param name="selfPosition"></param>
        /// <param name="selfVelocity"></param>
        /// <param name="targetPosition"></param>
        /// <param name="targetVelocity"></param>
        /// <returns>Whether the projectile is able to hit the target.</returns>
        public bool SuggestProjectileDirection(out Vector2 projectileDir, float projectileSpeed, Vector2 selfPosition, Vector2 selfVelocity, Vector2 targetPosition, Vector2 targetVelocity)
        {
            Vector2 relativePosition = targetPosition - selfPosition;
            Vector2 relativeVelocity = targetVelocity - selfVelocity;

            // simply return direction to the target if it isn't moving
            if (relativeVelocity == Vector2.zero)
            {
                projectileDir = relativePosition.normalized;
                return true;
            }
            
            // solve the time it would take for the projectile to hit the target
            // using the quadratic formula: t = (-b ± sqrt(b^2 - 4ac)) / 2a
            float a = Vector2.Dot(relativeVelocity, relativeVelocity) - projectileSpeed * projectileSpeed;
            float b = 2f * Vector2.Dot(relativeVelocity, relativePosition);
            float c = Vector2.Dot(relativePosition, relativePosition);
            
            float discriminant = b * b - 4f * a * c;
            float timeToHit = 0f;
            if (discriminant >= 0f && Mathf.Abs(a) > 1e-6f)
            {
                float sqrtDisc = Mathf.Sqrt(discriminant);
                float t1 = (-b + sqrtDisc) / (2f * a);
                float t2 = (-b - sqrtDisc) / (2f * a);
                
                if (t1 < 0f && t2 < 0f)
                {
                    projectileDir = relativePosition.normalized; // no solution, the projectile speed is too low to ever hit
                    return false;
                }
                
                // choose the smallest positive time to hit
                timeToHit = t1 > 0f && t2 > 0f
                    ? Mathf.Min(t1, t2) 
                    : Mathf.Max(t1, t2);
            }
            
            Vector2 aimPoint = targetPosition + targetVelocity * timeToHit;
            projectileDir = (aimPoint - selfPosition).normalized;
            return true;
        }
    }
}