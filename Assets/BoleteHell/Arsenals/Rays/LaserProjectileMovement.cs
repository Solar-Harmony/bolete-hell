using System;
using UnityEngine;

namespace BoleteHell.Arsenals.Rays
{
   [RequireComponent(typeof(Rigidbody2D))]
   public class LaserProjectileMovement : MonoBehaviour
   {
      private float _projectileSpeed;
      private Rigidbody2D _rb;
      private Vector3 _currentDirection;
      private bool _isColliding = false;
   
      private void Awake()
      {
         _rb = GetComponent<Rigidbody2D>();
      }

      public event Action<Collider2D> OnCollide;
   
      public void StartMovement(Vector2 direction, float speed)
      {
         _currentDirection = direction;
         _projectileSpeed = speed;
         _rb.linearVelocity = _currentDirection * _projectileSpeed;
      }
   
      public void SetDirection(Vector2 direction)
      {
         _currentDirection = direction;
         _rb.linearVelocity = _currentDirection * _projectileSpeed;
         float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
         transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
      }
   
      private void OnTriggerEnter2D(Collider2D other)
      {
         if (_isColliding)
            return;
         
         _isColliding = true;
      
         OnCollide?.Invoke(other);
      }
   
      private void OnTriggerExit2D(Collider2D other)
      {
         _isColliding = false;
      }

      public void RemoveCollideListeners()
      {
         OnCollide = null;
      }
   }
}
