using System;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Input.Controllers;
using UnityEngine.UIElements;

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
