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

   private void SetDirection(Vector2 direction)
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

   private bool _isColliding = false;
   
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (_isColliding)
         return;
      
      _isColliding = true;
      
      IHitHandler.Context context = new(other.gameObject, _instigator, gameObject, transform.position, _currentDirection, _laser);
      IHitHandler.TryHandleHit(context, resp =>
      {
         SetDirection(resp.Direction);
      
         if (resp.RequestDestroy)
         {
            Destroy(gameObject);
         }
      });
   }
   
   private void OnTriggerExit2D(Collider2D other)
   {
      _isColliding = false;
   }
}
