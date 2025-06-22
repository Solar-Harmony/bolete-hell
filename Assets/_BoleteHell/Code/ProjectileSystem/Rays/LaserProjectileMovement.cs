using System;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using Data.Rays;
using Shields;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Rigidbody2D))]
public class LaserProjectileMovement : MonoBehaviour
{
   private float _projectileSpeed;
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
      _rb.linearVelocity = _currentDirection * _projectileSpeed;
      float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
   }

   public void StartMovement(Vector2 direction, float speed, CombinedLaser laser, GameObject instigator = null)
   {
      _currentDirection = direction;
      _projectileSpeed = speed;
      _laser = laser;
      _instigator = instigator;
      _rb.linearVelocity = _currentDirection * _projectileSpeed;
   }

   // TODO: This should prolly be made into a generic hit handler, for bullets as well
   private void OnTriggerEnter2D(Collider2D other)
   {
      IHitHandler.Context context = new(other.gameObject, _instigator, gameObject, transform.position, _currentDirection, _laser);
      IHitHandler.Output output = IHitHandler.TryHandleHit(context);
      if (output != null)
      {
         SetDirection(output.Direction);
      }
   }
   public void DestroyProjectile()
   {
      Destroy(this.gameObject);
   }
}
