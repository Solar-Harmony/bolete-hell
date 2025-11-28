using System;
using UnityEngine;

namespace BoleteHell.Gameplay.Droppables
{
    [Serializable]
    public class DropSettings
    {
        //On pourra ajouter les listes d'objets a ajouter au drops possibles ici
        //Si un élite drop des upgrades unique
    
        [Header("Droplets settings")]
        public DropRangeContext dropletContext;

        [Header("Gold settings")]
        public DropRangeContext goldContext;
    }
}
