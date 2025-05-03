using UnityEngine;

namespace Input
{
    public class InputController : MonoBehaviour
    {
        private InputSystem_Actions _actions;
        
        private void Awake()
        {
            _actions = new InputSystem_Actions();
            _actions.Enable();
        }
        
        public Vector2 GetMovementDisplacement()
        {
            return _actions.Player.Move.ReadValue<Vector2>();
        }
    }
}