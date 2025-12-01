using UnityEngine;

namespace BoleteHell.AI.Services
{
    public interface ITargetingUtils
    {
        //Le expected détermine si je veux check pour "a line of sight" ou "n'a pas line of sight"
        //Vu que les conditionalGuard peuvent pas check si le retour est false
        public bool HasLineOfSight(GameObject self, GameObject target, float viewRange);

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
        public bool SuggestProjectileDirection(out Vector2 projectileDir, float projectileSpeed, Vector2 selfPosition, Vector2 selfVelocity, Vector2 targetPosition, Vector2 targetVelocity);
    }
}