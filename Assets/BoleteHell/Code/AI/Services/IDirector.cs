using BoleteHell.Code.Gameplay.Characters;

namespace BoleteHell.Code.AI.Services
{
    public interface IDirector
    {
        ISceneObject FindClosestShroom(Character self);
        ISceneObject FindTarget(Character self);
    }
}