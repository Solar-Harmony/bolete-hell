using Unity.Behavior;

namespace BoleteHell.Code.AI.Boilerplate
{
    public abstract class BoleteCondition : Condition, IRequestManualInject 
    {
        // Implement IsTrueImpl() instead.
        public sealed override bool IsTrue()
        {
            ((IRequestManualInject)this).InjectDependencies();
            return IsTrueImpl();
        }

        public abstract bool IsTrueImpl();

        bool IRequestManualInject.IsInjected { get; set; } = false;
    }
}