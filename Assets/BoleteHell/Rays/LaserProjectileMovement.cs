using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LaserProjectileMovement : MonoBehaviour
{
   private Rigidbody2D rb;
   private void Awake()
   {
      rb = GetComponent<Rigidbody2D>();
   }

   public void StartMovement(Vector2 direction, float speed)
   {
      rb.linearVelocity = direction * speed;
   }
}
