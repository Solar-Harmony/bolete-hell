using BoleteHell.Code.Gameplay.Character;

namespace BoleteHell.Code.AI.Services
{
    public interface IDirector
    {
        ICharacter FindTarget(Character self);
    }
}