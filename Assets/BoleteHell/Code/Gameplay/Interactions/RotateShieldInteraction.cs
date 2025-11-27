using System;
using BoleteHell.Code.Input.Controllers;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Interactions
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
