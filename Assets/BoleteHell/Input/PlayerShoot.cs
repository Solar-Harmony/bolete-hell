using BoleteHell.Utils;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoleteHell.Input
{
    /// <summary>
    /// Temporary
    /// </summary>
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField]
        private GameObject projPrefab;

        [SerializeField]
        private InputController input;
        
        [SerializeField]
        private Camera mainCamera;
        
        public float projectileSpeed = 1f;
        
        private void Update()
        {
            if (input.IsShooting)
            {
                Shoot();
            }
        }
        
        private void Shoot()
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
            Vector2 shootPos = transform.position + (Vector3)dir * 0.5f;
            var proj = Instantiate(projPrefab, shootPos, Quaternion.identity);
            var rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }
            else
            {
                Debug.LogError("Rigidbody2D component not found on the projectile prefab.");
            }
        }
    }
}