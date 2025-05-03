using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BulletHell.Scripts
{
    [RequireComponent(typeof(Player))]
    public class PlayerControls : MonoBehaviour,PlayerActions.ICombatActions
    {
        private PlayerActions _actions;

        private Vector3 _currentMovementValue;
        private Player player;
        private Vector3 mousePos2D;
        private bool isShootingRay;
        private bool isDrawingShield;
        private void Awake()
        {
            player = GetComponent<Player>();
            if (_actions == null)
            {
                _actions = new PlayerActions();
                _actions.Enable();
                _actions.Combat.SetCallbacks(this);
            }
            _actions.Combat.Enable();
        }

        private void Update()
        {
            transform.position += _currentMovementValue * (Time.deltaTime * player.movementSpeed);

            if (isShootingRay)
            {
                player.Shoot(mousePos2D);
            }
            if (isDrawingShield)
            {
                player.DrawShield(mousePos2D);
            }
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            _currentMovementValue = context.ReadValue<Vector2>();
        }

        public void OnLookAt(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(value);
            mousePos2D = new Vector3(mousePos.x, mousePos.y, 0);
            transform.up = mousePos2D - transform.position;
        }

        public void OnCycleShieldsForward(InputAction.CallbackContext context)
        {
            if(context.performed) 
                player.CycleShields(1);
        }

        public void OnCycleShieldsBackwards(InputAction.CallbackContext context)
        {
            if(context.performed) 
                player.CycleShields(-1);
        }

        public void OnShootRay(InputAction.CallbackContext context)
        {
            if (context.started)
                isShootingRay = true;
            else if (context.canceled)
                isShootingRay = false;
        }

        public void OnDrawShield(InputAction.CallbackContext context)
        {
            if (context.started)
                isDrawingShield = true;
            else if (context.canceled)
            {
                Debug.Log("Stopped holding left click");
                isDrawingShield = false;
                player.FinishShield();
            }

        }

        public void OnScrollWeapons(InputAction.CallbackContext context)
        {
            player.CycleWeapons((int)context.ReadValue<Vector2>().y);
        }
    }
}
