namespace BoleteHell.Code.AI.Boilerplate
{
    public abstract class BoleteCondition : Unity.Behavior.Condition, IRequestManualInject 
    {
        // Implement IsTrueImpl() instead.
        public sealed override bool IsTrue()
        {
            ((IRequestManualInject)this).InjectDependencies();
            return IsTrueImpl();
        }

        public abstract bool IsTrueImpl();

        bool IRequestManualInject.IsInjected { get; set; }
    }
}