using System;
using BoleteHell.Gameplay.InputControllers;
using UnityEngine;

namespace BoleteHell.Gameplay.Interactions
{
    [Serializable]
    public class RotateShieldInteraction : Interaction
    {
        public override void Interact(GameObject player)
        {
            player.GetComponent<ShieldInput>().CycleShields(1);
        }
    }
}
