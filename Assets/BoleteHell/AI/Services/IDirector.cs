using BoleteHell.Code.Audio.BoleteHell.Models;
using BoleteHell.Gameplay.Character;

namespace BoleteHell.AI.Services
{
    public interface IDirector
    {
        ISceneObject FindTarget(Character self);
    }
}