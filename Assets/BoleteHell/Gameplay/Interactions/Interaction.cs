using System;
using UnityEngine;

namespace BoleteHell.Gameplay.Interactions
{
    [Serializable]
    public abstract class Interaction
    {
        public abstract void Interact(GameObject player);
    }
}
