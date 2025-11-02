using System;
using UnityEngine;

public interface ILootable
{
    public DropContext dropContext { get; set; }
    public DropManager dropManager { get; set; }
}
