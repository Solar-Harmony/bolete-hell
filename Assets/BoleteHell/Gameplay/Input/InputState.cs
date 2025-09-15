using Zenject;

namespace BoleteHell.Gameplay.Input
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