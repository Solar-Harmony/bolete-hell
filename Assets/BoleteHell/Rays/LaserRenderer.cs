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
        [field:SerializeField] public float RayWidth { get; private set; } = 0.2f;
        //Spécifique au projectile lasers
        [field:SerializeField]public float LaserLenght { get; private set; } = 0.3f;
        private LineRenderer _laserRenderer;
        
        private LaserProjectileMovement _movement;
        private CapsuleCollider2D _capsuleCollider;
        private Rigidbody2D _rb;
        
        private LineRendererPool _parentPool;
        private const float AdjustedColliderLenght = 0.15f;
        private bool _isProjectile;

        private void Awake()
        {
            _laserRenderer = GetComponent<LineRenderer>();
            _movement = GetComponent<LaserProjectileMovement>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
            
            _capsuleCollider.enabled = false;
            _movement.enabled = false;

        }

        public void DrawRay(List<Vector3> positions, Color color, float lifeTime,RayCannonFiringLogic logic)
        {
            gameObject.SetActive(true);
            _laserRenderer.positionCount = positions.Count;
            _laserRenderer.SetPositions(positions.ToArray());
            
            _laserRenderer.startColor = color;
            _laserRenderer.endColor = color;
            StartCoroutine(Lifetime(lifeTime,logic));
        }

        public void SetupProjectileLaser(float refractiveIndex,Vector2 direction)
        {
            _isProjectile = true;
            _laserRenderer.useWorldSpace = false;

            _movement.enabled = true;
            _movement.StartMovement(direction,refractiveIndex);
            
            _capsuleCollider.direction = CapsuleDirection2D.Vertical;
            _capsuleCollider.size = new Vector2(RayWidth, LaserLenght + AdjustedColliderLenght);
            _capsuleCollider.offset = new Vector2(0, LaserLenght / 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
            _capsuleCollider.enabled = true;

            _laserRenderer.numCapVertices = 10;
        }

        private IEnumerator Lifetime(float time,RayCannonFiringLogic logic)
        {
            yield return new WaitForSeconds(time);
            ResetLaser(logic);
        }

        //Pourrais peut-être avoir un renderer pour les laserbeams et un renderer pour les projectile laser
        private void ResetLaser(RayCannonFiringLogic logic)
        {
            //gameObject.SetActive(false);
            logic.OnReset(this);
            
            if (!_isProjectile) return;
            _laserRenderer.useWorldSpace = true;
            _laserRenderer.positionCount = 0;
            _capsuleCollider.enabled = false;
            _movement.enabled = false;
            _rb.linearVelocity = Vector2.zero;
            _laserRenderer.numCapVertices = 0;
            _isProjectile = false;

        }

    }
}