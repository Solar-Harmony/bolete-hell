using BoleteHell.Code.Gameplay.Character;

namespace BoleteHell.Code.AI.Services
{
    public interface IDirector
    {
        ISceneObject FindTarget(Character self);
    }
}