using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lasers
{
    //Je voullais rendre le laserRenderer fonctionnel pour les beams et les projectiles
    [RequireComponent(typeof(LaserProjectileMovement),typeof(CapsuleCollider2D),typeof(LineRenderer))]
    public class LaserRenderer : MonoBehaviour
    {
        private LineRenderer laserRenderer;
        
        private LaserProjectileMovement movement;
        private CapsuleCollider2D collider;
        private Rigidbody2D rb;
        
        private LineRendererPool _parentPool;
        public int Id { get; private set; }

        private void Awake()
        {
            laserRenderer = GetComponent<LineRenderer>();
            movement = GetComponent<LaserProjectileMovement>();
            collider = GetComponent<CapsuleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            
            collider.enabled = false;
            movement.enabled = false;

        }

        public void Init(int id)
        {
            Id = id;
        }

        public void DrawRay(List<Vector3> positions, Color color)
        {
            laserRenderer.positionCount = positions.Count;
            laserRenderer.SetPosition(0, positions[0]);

            for (var i = 1; i < positions.Count; i++)
            {
                var pos = positions[i];
                laserRenderer.SetPosition(i, pos);
            }
            
            laserRenderer.startColor = color;
            laserRenderer.endColor = color;
        }

        public void SetupProjectileLaser(float laserWidth,float laserLenght,Vector2 direction,float laserSpeed)
        {
            movement.enabled = true;
            movement.StartMovement(direction,laserSpeed);

            collider.direction = CapsuleDirection2D.Vertical;
            collider.size = new Vector2(laserWidth, laserLenght +0.15f);
            collider.offset = new Vector2(0, laserLenght / 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float offset = -90f; 
            transform.rotation = Quaternion.Euler(0, 0, angle + offset);
            collider.enabled = true;

            laserRenderer.numCapVertices = 10;

        }


        private void Reset()
        {
            Debug.Log($"Reseting lineRenderer {Id}");
            laserRenderer.positionCount = 0;
            gameObject.SetActive(false);
            collider.enabled = false;
            movement.enabled = false;
            rb.linearVelocity = Vector2.zero;
            laserRenderer.numCapVertices = 0;
        }

    }
}