using BoleteHell.Utils;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BoleteHell.Input
{
    // just dummy
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
            if (input.IsShooting())
            {
                Shoot();
            }
        }
        
        private void Shoot()
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var proj = Instantiate(projPrefab, mousePos, Quaternion.identity);
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