using System.Collections;
using System.Collections.Generic;
using Data.Rays;
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
        [field:SerializeField]public float LaserLength { get; private set; } = 0.3f;
        private LineRenderer _lineRenderer;
        
        private LaserProjectileMovement _movement;
        private CapsuleCollider2D _capsuleCollider;
        private Rigidbody2D _rb;
        
        private LaserRendererPool _parentPool;
        private const float AdjustedColliderLenght = 0.15f;
        private bool _isProjectile;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _movement = GetComponent<LaserProjectileMovement>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
            
            _capsuleCollider.enabled = false;
            _movement.enabled = false;
        }

        public void DrawRay(List<Vector3> positions, Color color, float lifeTime,FiringLogic logic)
        {
            gameObject.SetActive(true);
            _lineRenderer.positionCount = positions.Count;
            _lineRenderer.SetPositions(positions.ToArray());
            
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
            StartCoroutine(Lifetime(lifeTime,logic));
        }

        public void SetupProjectileLaser(float refractiveIndex,Vector2 direction,CombinedLaser laser)
        {
            _isProjectile = true;
            _lineRenderer.useWorldSpace = false;

            _movement.enabled = true;
            _movement.StartMovement(direction,refractiveIndex,laser);
            
            _capsuleCollider.direction = CapsuleDirection2D.Vertical;
            _capsuleCollider.size = new Vector2(RayWidth, LaserLength + AdjustedColliderLenght);
            _capsuleCollider.offset = new Vector2(0, LaserLength / 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
            _capsuleCollider.enabled = true;

            _lineRenderer.numCapVertices = 10;
        }

        private IEnumerator Lifetime(float time,FiringLogic logic)
        {
            yield return new WaitForSeconds(time);
            ResetLaser(logic);
        }

        //Pourrais peut-être avoir un renderer pour les laserbeams et un renderer pour les projectile laser
        private void ResetLaser(FiringLogic logic)
        {
            LaserRendererPool.Instance.Release(this);
            
            if (!_isProjectile) return;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.positionCount = 0;
            _capsuleCollider.enabled = false;
            _movement.enabled = false;
            _rb.linearVelocity = Vector2.zero;
            _lineRenderer.numCapVertices = 0;
            _isProjectile = false;
        }
    }
}