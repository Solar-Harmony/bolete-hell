using System;
using Shields;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LaserProjectileMovement : MonoBehaviour
{
   [SerializeField] private float projectileSpeed = 10f;
   private Rigidbody2D rb;
   private Vector3 currentDirection;
   private float refractiveIndex;
   private void Awake()
   {
      rb = GetComponent<Rigidbody2D>();
   }

   public void StartMovement(Vector2 direction, float lightRefractiveIndex )
   {
      currentDirection = direction;
      refractiveIndex = lightRefractiveIndex;
      rb.linearVelocity = currentDirection * projectileSpeed;
   }

   //Devrait peut-être pas être dans le script de mouvement
   private void OnTriggerEnter2D(Collider2D other)
   {
      Debug.Log($"Touched {other.name}");
      if (!other.transform.parent) return;
      
      if (other.transform.parent.gameObject.TryGetComponent(out Shield lineHit))
      {
         OnHitShield(lineHit);
      }
      else if (other.CompareTag("Enemy"))
      {
         OnHitEnemy(other.gameObject);
      }
   }

   private void OnHitEnemy(GameObject enemy)
   {
      Debug.Log($"Hit {enemy.name}");
   }

   //Devrait être dans le laserProjectileLogic
   private void OnHitShield(Shield shieldHit)
   {
      RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position , currentDirection, Mathf.Infinity);
      
      if (!hit) return;
         
      Vector3 newDirection = shieldHit.OnRayHitLine(currentDirection, hit, refractiveIndex);
      currentDirection = newDirection;
      rb.linearVelocity = currentDirection * projectileSpeed;
      float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
   }
}
