using JetBrains.Annotations;
using Zenject;

namespace BoleteHell.Gameplay.Input
{
    /// <summary>
    /// The only purpose of this class is to manage the lifetime of the generated input actions,
    /// since it's shared across multiple input systems.
    /// </summary>
    [UsedImplicitly]
    public class InputActionsWrapper : IInputActionsWrapper, IInitializable
    {
        public InputSystem_Actions Actions { get; private set; }

        public void Initialize()
        {
            Actions = new InputSystem_Actions();
            Actions.Enable();
        }
    }
}