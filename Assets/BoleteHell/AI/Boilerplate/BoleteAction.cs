using BoleteHell.Utils;
using Unity.Behavior;

namespace BoleteHell.AI.Boilerplate
{
    public abstract class BoleteAction : Action
    {
        // Implement OnStartImpl() instead.
        protected sealed override Status OnStart()
        {
            ServiceLocator.Inject(this);
            return OnStartImpl();
        }

        protected virtual Status OnStartImpl()
        {
            return Status.Running;
        }
    }
}