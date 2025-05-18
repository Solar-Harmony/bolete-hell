using System.Collections;
using System.Collections.Generic;
using Lasers;
using UnityEngine;

namespace BoleteHell.Rays
{
    //Je voullais rendre le laserRenderer fonctionnel pour les beams et les projectiles
    [RequireComponent(typeof(LaserProjectileMovement),typeof(CapsuleCollider2D),typeof(LineRenderer))]
    public class LaserRenderer : MonoBehaviour
    {
        private LineRenderer laserRenderer;
        
        private LaserProjectileMovement movement;
        private CapsuleCollider2D capsuleCollider;
        private Rigidbody2D rb;
        
        private LineRendererPool _parentPool;
        private const float AdjustedColliderLenght = 0.15f;
        private bool isProjectile;

        private void Awake()
        {
            laserRenderer = GetComponent<LineRenderer>();
            movement = GetComponent<LaserProjectileMovement>();
            capsuleCollider = GetComponent<CapsuleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            
            capsuleCollider.enabled = false;
            movement.enabled = false;

        }

        public void DrawRay(List<Vector3> positions, Color color, float lifeTime,RayCannonFiringLogic logic)
        {
            gameObject.SetActive(true);
            laserRenderer.positionCount = positions.Count;
            laserRenderer.SetPositions(positions.ToArray());
            
            laserRenderer.startColor = color;
            laserRenderer.endColor = color;
            StartCoroutine(Lifetime(lifeTime,logic));
        }

        public void SetupProjectileLaser(LaserProjectileData laserData,Vector2 direction,float laserSpeed)
        {
            isProjectile = true;
            laserRenderer.useWorldSpace = false;

            movement.enabled = true;
            movement.StartMovement(direction,laserSpeed,laserData.LightRefractiveIndex);

            capsuleCollider.direction = CapsuleDirection2D.Vertical;
            capsuleCollider.size = new Vector2(laserData.rayWidth, laserData.laserLenght + AdjustedColliderLenght);
            capsuleCollider.offset = new Vector2(0, laserData.laserLenght / 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
            capsuleCollider.enabled = true;

            laserRenderer.numCapVertices = 10;
        }

        private IEnumerator Lifetime(float time,RayCannonFiringLogic logic)
        {
            yield return new WaitForSeconds(time);
            Reset(logic);
        }

        //Pourrais peut-être avoir un renderer pour les laserbeams et un renderer pour les projectile laser
        private void Reset(RayCannonFiringLogic logic)
        {
            gameObject.SetActive(false);
            logic.OnReset(this);
            
            if (!isProjectile) return;
            laserRenderer.useWorldSpace = true;
            laserRenderer.positionCount = 0;
            capsuleCollider.enabled = false;
            movement.enabled = false;
            rb.linearVelocity = Vector2.zero;
            laserRenderer.numCapVertices = 0;
            isProjectile = false;

        }

    }
}