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
   private float _refractiveIndex;
   private CombinedLaser _laser;
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

   public void StartMovement(Vector2 direction, float lightRefractiveIndex, CombinedLaser laser)
   {
      Debug.Log("Started movement");
      _currentDirection = direction;
      _refractiveIndex = lightRefractiveIndex;
      _rb.linearVelocity = _currentDirection * projectileSpeed;
      _laser = laser;
   }

   //Devrait peut-être pas être dans le script de mouvement
   private void OnTriggerEnter2D(Collider2D other)
   {
      Debug.Log($"hit {other.name}");
      // if (other.transform.CompareTag("Shield"))
      // {
      //    if (other.transform.parent.gameObject.TryGetComponent(out Shield lineHit))
      //    {
      //       OnHitShield(lineHit);
      //    }
      // }
      // else if (other.transform.gameObject.TryGetComponent(out Health health))
      // {
      //    OnHitEnemy(transform.position,health);
      // }
      
      IHitHandler handler = other.GetComponent<IHitHandler>() 
                            ?? other.GetComponentInParent<IHitHandler>(); // TODO : needed because of shield, child colliders are not registered to composite collider correctly but i couldn't get it working
      if (handler == null)
      {
         Debug.LogWarning($"No IHitHandler found on {other.name}. Ignored hit.");
         return;
      }
      IHitHandler.Context context = new(gameObject, transform.position, _currentDirection, _laser);
      handler.OnHit(context);
   }

   // private void OnHitEnemy(Vector2 position,Health health)
   // {
   //    _laser.CombinedEffect(position,health);
   // }

   //Devrait être dans le laserProjectileLogic
   private void OnHitShield(Shield shieldHit)
   {
      LayerMask layerMask = ~LayerMask.GetMask("Projectile");
      RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position , _currentDirection, Mathf.Infinity,layerMask);

      Debug.Log($"Raycast hit: {hit.collider?.name ?? "nothing"} at position {hit.point}");
      
      if (!hit) return;
         
      Vector3 newDirection = shieldHit.OnRayHitLine(_currentDirection, hit, _refractiveIndex);
      _currentDirection = newDirection;
      _rb.linearVelocity = _currentDirection * projectileSpeed;
      float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle + -90f);
   }
}
