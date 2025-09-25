using System;
using BoleteHell.Code.Gameplay.Character;

namespace BoleteHell.Code.Gameplay.Interactions
{
    [Serializable]
    public abstract class Interaction
    {
        public abstract void Interact(Player player);
    }
}
