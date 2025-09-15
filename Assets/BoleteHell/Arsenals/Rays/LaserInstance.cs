using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Arsenals.Rays
{
    //TODO: Va devoir être cleaned up et séparer
    [RequireComponent(typeof(LaserProjectileMovement), typeof(CapsuleCollider2D),typeof(LineRenderer))]
    public class LaserInstance : MonoBehaviour//, IStatusEffectTarget, IMovable, IDamageDealer
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
        
        public bool isProjectile;
        public float MovementSpeed { get; set; }
        public float DamageMultiplier { get; set; } = 1;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _movement = GetComponent<LaserProjectileMovement>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
            
            _capsuleCollider.enabled = false;
            _movement.enabled = false;
        }

        public void DrawRay(List<Vector3> positions, Color color, float lifeTime)
        {

            _lineRenderer.positionCount = positions.Count;
            _lineRenderer.SetPositions(positions.ToArray());
            
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
            StartCoroutine(Lifetime(lifeTime));
        }

        public LaserProjectileMovement SetupProjectileLaser(Vector2 direction, float speed)
        {
            MovementSpeed = speed;
            isProjectile = true;
            _lineRenderer.useWorldSpace = false;

            _movement.enabled = true;
            _movement.StartMovement(direction, MovementSpeed);
            
            _capsuleCollider.direction = CapsuleDirection2D.Vertical;
            _capsuleCollider.size = new Vector2(RayWidth, LaserLength + AdjustedColliderLenght);
            _capsuleCollider.offset = new Vector2(0, LaserLength / 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
            _capsuleCollider.enabled = true;

            _lineRenderer.numCapVertices = 10;

            return _movement;
        }

        private IEnumerator Lifetime(float time)
        {
            yield return new WaitForSeconds(time);
            ResetLaser();
        }

        //Pourrais peut-être avoir un renderer pour les laserbeams et un renderer pour les projectile laser
        public void ResetLaser()
        {
            LaserRendererPool.Instance.Release(this);
            if (!isProjectile) return;
            
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.positionCount = 0;
            _capsuleCollider.enabled = false;
            _movement.enabled = false;
            _rb.linearVelocity = Vector2.zero;
            _lineRenderer.numCapVertices = 0;
            isProjectile = false;
            _movement.RemoveCollideListeners();
            MovementSpeed = 0;
            DamageMultiplier = 1;
        }

        public bool IsValid => true;
    }
}