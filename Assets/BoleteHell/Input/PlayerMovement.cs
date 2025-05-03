using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private InputController input;
        
        public float speed = 5f;

        private void Start()
        {
            this.AssertNotNull(input);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            Vector2 delta = input.GetMovementDisplacement();
            transform.Translate(delta * speed * Time.deltaTime);
        }
    }
}