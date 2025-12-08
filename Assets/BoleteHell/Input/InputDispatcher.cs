using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace BoleteHell.Code.Input
{
    /// <summary>
    /// Binds input actions to events and provides polled input.
    /// </summary>
    public class InputDispatcher : IInputDispatcher, IInitializable, IDisposable
    {
        [Inject]
        private IInputActionsWrapper _actionsProvider;
        
        private InputSystem_Actions Actions => _actionsProvider.Actions;
        
        [Inject]
        private Camera _camera;

        // polled input
        public bool IsMoving => MovementDisplacement != Vector2.zero;
        public bool IsBoosting => Actions.Player.Sprint.IsPressed();
        public bool IsChargingShot => Actions.Player.Shoot.IsPressed();
        public bool IsDrawingShield => Actions.Player.DrawShield.IsPressed();
        public Vector2 MovementDisplacement => Actions.Player.Move.ReadValue<Vector2>();
        public Vector2 MousePosition => Actions.Player.MousePos2D.ReadValue<Vector2>();
        public Vector2 WorldMousePosition => _camera.ScreenToWorldPoint(MousePosition);
        
        // input events
        public event Action OnShieldStart;
        public event Action OnShieldEnd;
        public event Action OnShoot;
        public event Action OnInteract;
        public event Action OnDodge;
        public event Action<int> OnCycleWeapons;
        public event Action<int> OnCycleShield;
        public event Action OnReloadGame;
        
        public void Initialize()
        {
            if (Actions?.Player == null) return;
            
            Actions.Player.Shoot.canceled += OnShootHandler;
            Actions.Player.Interact.canceled += OnInteractHandler;
            Actions.Player.Dodge.canceled += OnDodgeHandler;
            Actions.Player.CycleNextShield.performed += OnCycleNextShieldHandler;
            Actions.Player.CyclePreviousShield.performed += OnCyclePreviousShieldHandler;
            Actions.Player.DrawShield.started += OnShieldStartHandler;
            Actions.Player.DrawShield.canceled += OnShieldEndHandler;
            Actions.Player.CycleWeapons.performed += OnCycleWeaponsHandler;
            Actions.Player.ReloadGame.performed += OnReloadGameHandler;
        }
        
        public void Dispose()
        {
            if (Actions?.Player == null) return;
            
            Actions.Player.Shoot.canceled -= OnShootHandler;
            Actions.Player.Interact.canceled -= OnInteractHandler;
            Actions.Player.Dodge.canceled -= OnDodgeHandler;
            Actions.Player.CycleNextShield.performed -= OnCycleNextShieldHandler;
            Actions.Player.CyclePreviousShield.performed -= OnCyclePreviousShieldHandler;
            Actions.Player.DrawShield.started -= OnShieldStartHandler;
            Actions.Player.DrawShield.canceled -= OnShieldEndHandler;
            Actions.Player.CycleWeapons.performed -= OnCycleWeaponsHandler;
            Actions.Player.ReloadGame.performed -= OnReloadGameHandler;
        }
        
        private void OnShootHandler(InputAction.CallbackContext ctx) => OnShoot?.Invoke();
        private void OnInteractHandler(InputAction.CallbackContext ctx) => OnInteract?.Invoke();
        private void OnDodgeHandler(InputAction.CallbackContext ctx) => OnDodge?.Invoke();
        private void OnCycleNextShieldHandler(InputAction.CallbackContext ctx) => OnCycleShield?.Invoke(1);
        private void OnCyclePreviousShieldHandler(InputAction.CallbackContext ctx) => OnCycleShield?.Invoke(-1);
        private void OnShieldStartHandler(InputAction.CallbackContext ctx) => OnShieldStart?.Invoke();
        private void OnShieldEndHandler(InputAction.CallbackContext ctx) => OnShieldEnd?.Invoke();
        private void OnReloadGameHandler(InputAction.CallbackContext ctx) => OnReloadGame?.Invoke();
        private void OnCycleWeaponsHandler(InputAction.CallbackContext ctx)
        {
            float y = ctx.ReadValue<Vector2>().y;
            if (Mathf.Approximately(y, 0))
                return;

            int direction = y > 0 ? 1 : -1;
            OnCycleWeapons?.Invoke(direction);
        }
    }
}