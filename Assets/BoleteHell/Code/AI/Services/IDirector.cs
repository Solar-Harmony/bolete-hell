using BoleteHell.Code.Gameplay.Characters;

namespace BoleteHell.Code.AI.Services
{
    public interface IDirector
    {
        ISceneObject FindWeakestAlly(Character self);
        ISceneObject FindNearestAlly(Character self);
        ISceneObject FindNearestTarget(Character self);
    }
}