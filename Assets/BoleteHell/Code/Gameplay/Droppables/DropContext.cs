using System;
using UnityEngine;

[Serializable]
public class DropContext
{
    //On pourra ajouter les listes d'objets a ajouter au drops possibles ici
    //Si un élite drop des upgrades unique
    
    [Header("Droplets settings")]
    public DropRangeContext dropletContext;

    [Header("Gold settings")]
    public DropRangeContext goldContext;
}
