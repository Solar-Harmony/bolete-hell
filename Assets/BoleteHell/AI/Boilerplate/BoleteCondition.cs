using BoleteHell.Utils;

namespace BoleteHell.AI.Boilerplate
{
    public abstract class BoleteCondition : Unity.Behavior.Condition 
    {
        // Implement IsTrueImpl() instead.
        public sealed override bool IsTrue()
        {
            ServiceLocator.Inject(this);
            return IsTrueImpl();
        }

        public abstract bool IsTrueImpl();
    }
}