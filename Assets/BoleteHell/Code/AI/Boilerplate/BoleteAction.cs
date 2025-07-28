using Unity.Behavior;

namespace BoleteHell.Code.AI.Boilerplate
{
    public abstract class BoleteAction : Action, IRequestManualInject
    {
        // Implement OnStartImpl() instead.
        protected sealed override Status OnStart()
        {
            ((IRequestManualInject)this).InjectDependencies();
            return OnStartImpl();
        }

        protected virtual Status OnStartImpl()
        {
            return Status.Running;
        }

        bool IRequestManualInject.IsInjected { get; set; } = false;
    }
}