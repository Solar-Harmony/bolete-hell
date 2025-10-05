using System;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Input.Controllers;

namespace BoleteHell.Code.Gameplay.Interactions
{
    [Serializable]
    public class RotateShieldInteraction: Interaction
    {
        
        public override void Interact(Player player)
        {
            player.GetComponent<ShieldInput>().CycleShields(1);
        }
    }
}
