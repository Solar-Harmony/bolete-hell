using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BulletHell.Scripts
{
    [RequireComponent(typeof(Player))]
    //Remove all this shit and Check in the components that need to know because rn this forces me to have way too many public variables and methods in Player
    public class PlayerControls : MonoBehaviour,PlayerActions.ICombatActions
    {
        private PlayerActions _actions;

        private Vector3 _currentMovementValue;
        private Player player;
        public Vector3 mousePos2D;
        public bool isShootingRay;
        public bool isDrawingShield;
        
        [HideInInspector]
        public UnityEvent<InputAction.CallbackContext> startedShooting;
        [HideInInspector]
        public UnityEvent<InputAction.CallbackContext> endedShooting;
        [HideInInspector]
        public UnityEvent<InputAction.CallbackContext> startedDrawingShield;
        [HideInInspector]
        public UnityEvent<InputAction.CallbackContext> endedDrawingShield;
        [HideInInspector]
        public UnityEvent<int> scrolled;
        [HideInInspector]
        public UnityEvent<int> shieldCycled;
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
                shieldCycled.Invoke(1);
        }

        public void OnCycleShieldsBackwards(InputAction.CallbackContext context)
        {
            if(context.performed) 
                shieldCycled.Invoke(-1);
        }

        public void OnShootRay(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isShootingRay = true;
                startedShooting.Invoke(context);
            }
            else if (context.canceled)
            {
                isShootingRay = false;
                endedShooting.Invoke(context);
            }

            
        }

        public void OnDrawShield(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isDrawingShield = true;
                startedDrawingShield.Invoke(context);
            }

            else if (context.canceled)
            {
                isDrawingShield = false;
                endedDrawingShield.Invoke(context);
            }

        }

        public void OnScrollWeapons(InputAction.CallbackContext context)
        {
            if(context.performed) 
                scrolled.Invoke((int)context.ReadValue<Vector2>().y); 
        }
    }
}
