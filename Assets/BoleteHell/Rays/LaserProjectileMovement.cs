using System;
using Shields;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LaserProjectileMovement : MonoBehaviour
{
   private Rigidbody2D rb;
   private Vector3 currentDirection;
   private float refractiveIndex;
   private float currentSpeed;
   private void Awake()
   {
      rb = GetComponent<Rigidbody2D>();
   }

   public void StartMovement(Vector2 direction, float speed,float lightRefractiveIndex )
   {
      currentDirection = direction;
      currentSpeed = speed;
      refractiveIndex = lightRefractiveIndex;
      rb.linearVelocity = currentDirection * currentSpeed;
   }

   //Devrait peut-être pas être dans le script de mouvement
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.transform.parent) return;
      
      if (other.transform.parent.gameObject.TryGetComponent(out Line lineHit))
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
   private void OnHitShield(Line lineHit)
   {
      RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position , currentDirection, Mathf.Infinity);
      
      if (!hit) return;
         
      Vector3 newDirection = lineHit.OnRayHitLine(currentDirection, hit, refractiveIndex);
      currentDirection = newDirection;
      rb.linearVelocity = currentDirection * currentSpeed;
      float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
   }
}
