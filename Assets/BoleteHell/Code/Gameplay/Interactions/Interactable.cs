using System;
using System.Collections.Generic;
using BoleteHell.Code.Gameplay.Character;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Interactions
{
    public class Interactable : MonoBehaviour
    {
        [SerializeReference]
        private List<Interaction> interactions = new ();

        public void Interact(Player player)
        {
            foreach (Interaction interaction in interactions)
            {
                interaction.Interact(player);
            }
        }
    }
}
