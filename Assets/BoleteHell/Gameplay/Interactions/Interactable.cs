using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Gameplay.Interactions
{
    public class Interactable : MonoBehaviour
    {
        [SerializeReference]
        private List<Interaction> interactions = new ();

        public void Interact(GameObject player)
        {
            foreach (Interaction interaction in interactions)
            {
                interaction.Interact(player);
            }
        }
    }
}
