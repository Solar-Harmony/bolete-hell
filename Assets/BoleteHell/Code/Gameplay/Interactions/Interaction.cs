using System;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Interactions
{
    [Serializable]
    public abstract class Interaction
    {
        public abstract void Interact(GameObject player);
    }
}
