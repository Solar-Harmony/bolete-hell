using System.Collections;
using System.Collections.Generic;
using Lasers;
using UnityEngine;
using Ray = Lasers.Ray;

namespace BoleteHell.Rays
{
    //Je voullais rendre le laserRenderer fonctionnel pour les beams et les projectiles
    [RequireComponent(typeof(LaserProjectileMovement),typeof(CapsuleCollider2D),typeof(LineRenderer))]
    public class LaserRenderer : MonoBehaviour
    {
        private LineRenderer laserRenderer;
        
        private LaserProjectileMovement movement;
        private CapsuleCollider2D cCollider;
        private Rigidbody2D rb;
        
        private LineRendererPool _parentPool;

        private void Awake()
        {
            laserRenderer = GetComponent<LineRenderer>();
            movement = GetComponent<LaserProjectileMovement>();
            cCollider = GetComponent<CapsuleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            
            cCollider.enabled = false;
            movement.enabled = false;

        }

        public void DrawRay(List<Vector3> positions, Color color, float lifeTime)
        {
            gameObject.SetActive(true);
            laserRenderer.positionCount = positions.Count;
            laserRenderer.SetPositions(positions.ToArray());
            
            laserRenderer.startColor = color;
            laserRenderer.endColor = color;
            StartCoroutine(Lifetime(lifeTime));
        }

        public void SetupProjectileLaser(Ray ray,Vector2 direction,float laserSpeed,LaserProjectileLogic logic)
        {
            laserRenderer.useWorldSpace = false;

            movement.enabled = true;
            movement.StartMovement(direction,laserSpeed,ray.LightRefractiveIndex);

            cCollider.direction = CapsuleDirection2D.Vertical;
            //Le plus 0.15 est un nombre magique qui permettait de fit le collider sinon il correspond pas a 100%
            cCollider.size = new Vector2(ray.rayWidth, ray.raylength + 0.15f);
            cCollider.offset = new Vector2(0, ray.raylength / 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
            cCollider.enabled = true;

            laserRenderer.numCapVertices = 10;
        }

        private IEnumerator Lifetime(float time)
        {
            yield return new WaitForSeconds(time);
            Reset();
        }

        //Pourrais peut-être avoir un renderer pour les laserbeams et un renderer pour les projectile laser
        public void Reset()
        {
            laserRenderer.useWorldSpace = true;
            laserRenderer.positionCount = 0;
            gameObject.SetActive(false);
            cCollider.enabled = false;
            movement.enabled = false;
            rb.linearVelocity = Vector2.zero;
            laserRenderer.numCapVertices = 0;
            LineRendererPool.Instance.Release(this);
        }

    }
}