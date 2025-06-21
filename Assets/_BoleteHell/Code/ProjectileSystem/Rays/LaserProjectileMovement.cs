using System;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using Data.Rays;
using Shields;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LaserProjectileMovement : MonoBehaviour
{
   [SerializeField] private float projectileSpeed = 10f;
   private Rigidbody2D _rb;
   private Vector3 _currentDirection;
   private CombinedLaser _laser;
   private GameObject _instigator;
   private void Awake()
   {
      _rb = GetComponent<Rigidbody2D>();
   }
   
   public void SetDirection(Vector2 direction)
   {
      _currentDirection = direction;
      _rb.linearVelocity = _currentDirection * projectileSpeed;
      float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
   }

   public void StartMovement(Vector2 direction, CombinedLaser laser, GameObject instigator = null)
   {
      _instigator = instigator;
      _currentDirection = direction;
      _rb.linearVelocity = _currentDirection * projectileSpeed;
      _laser = laser;
   }

   // TODO: This should prolly be made into a generic hit handler, for bullets as well
   private void OnTriggerEnter2D(Collider2D other)
   {
      // always ignore hits with the instigator (for now)
      if (other.gameObject == _instigator)
         return;
      
      // ignore hits with other projectiles (for now)
      if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
         return;
      
      IHitHandler handler = other.GetComponent<IHitHandler>() 
                            ?? other.GetComponentInParent<IHitHandler>(); // TODO : needed because of shield, child colliders are not registered to composite collider correctly but i couldn't get it working
      if (handler == null)
         return;
      
      IHitHandler.Context context = new(gameObject, transform.position, _currentDirection, _laser);
      handler.OnHit(context);
   }
}
