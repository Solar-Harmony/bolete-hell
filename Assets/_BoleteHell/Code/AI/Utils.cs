using UnityEngine;

namespace _BoleteHell.Code.AI
{
    // TODO: Move somewhere better
    // TODO: This is really not performant, every AI does a raycast every frame lol
    public static class AIUtils
    {
        public static bool HasLineOfSight(GameObject self, GameObject agent, float viewRange)
        {
            if (!self || !agent)
                return false;
        
            Vector3 direction = agent.transform.position - self.transform.position;
            LayerMask layerMask = ~LayerMask.GetMask("PlayerEnemy", "Projectile");
            
            // TODO: Use circle cast for accounting for laser width
            RaycastHit2D hit = Physics2D.Raycast(self.transform.position, direction.normalized, viewRange, layerMask);

            return hit.collider.gameObject == agent;
        }
    }
}