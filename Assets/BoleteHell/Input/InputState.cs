using BoleteHell.Code.Input;
using Zenject;

namespace BoleteHell.Code.Gameplay.Input
{
    public class InputState : IInputState
    {
        [Inject]
        private IInputActionsWrapper _actionsWrapper;
        
        public void EnableInput()
        {
            _actionsWrapper.Actions.Enable();
        }

        public void DisableInput()
        {
            _actionsWrapper.Actions.Disable();
        }
    }
}