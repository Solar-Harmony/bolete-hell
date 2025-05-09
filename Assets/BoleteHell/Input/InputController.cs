using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputController : MonoBehaviour
    {
        private InputSystem_Actions _actions;
        [SerializeField] private Camera _camera;
        
        public bool IsMoving => GetMovementDisplacement() != Vector2.zero;
        public bool IsBoosting => _actions.Player.Sprint.IsPressed();
        public bool IsShooting => _actions.Player.Shoot.IsPressed();
        public bool IsDrawingShield => _actions.Player.DrawShield.IsPressed();

        public Vector2 MousePosition => _actions.Player.MousePos2D.ReadValue<Vector2>();
        public Vector2 WorldMousePosition
        {
            get
            {
                Vector2 value = _actions.Player.MousePos2D.ReadValue<Vector2>();
                Vector3 mousePos = _camera.ScreenToWorldPoint(value);
                Vector3 pos2D = new Vector3(mousePos.x, mousePos.y, 0);
                transform.up = pos2D - transform.position;
                return pos2D;
            }
        }

        private void Awake()
        {
            _actions = new InputSystem_Actions();
            _actions.Enable();
            _actions.Player.Shoot.started += ctx => OnShootStarted?.Invoke();
            _actions.Player.Shoot.canceled += ctx => OnShootEnded?.Invoke();
            _actions.Player.CycleNextShield.performed += ctx => OnCycleShield?.Invoke(1);
            _actions.Player.CyclePreviousShield.performed += ctx => OnCycleShield?.Invoke(-1);
            _actions.Player.DrawShield.started += ctx => OnShieldStart?.Invoke();
            _actions.Player.DrawShield.canceled += ctx => OnShieldEnd?.Invoke();
            _actions.Player.CycleWeapons.performed +=
                ctx => OnCycleWeapons?.Invoke(ctx.ReadValue<Vector2>().y > 0 ? 1 : -1);
        }

        public event Action OnShieldStart;
        public event Action OnShieldEnd;

        public event Action OnShootStarted;
        public event Action OnShootEnded;
        public event Action<int> OnCycleWeapons;
        public event Action<int> OnCycleShield;

        public Vector2 GetMovementDisplacement()
        {
            return _actions.Player.Move.ReadValue<Vector2>();
        }
    }
}