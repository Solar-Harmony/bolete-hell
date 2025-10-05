using BoleteHell.Code.Gameplay.Characters;

namespace BoleteHell.Code.AI.Services
{
    public interface IDirector
    {
        ISceneObject FindTarget(Character self);
    }
}