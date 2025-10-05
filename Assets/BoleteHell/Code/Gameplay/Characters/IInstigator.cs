using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Characters
{
    /// <summary>
    /// The source of an attack.
    /// </summary>
    public interface IInstigator : IDamageDealer, IFaction
    {
        Health Health { get; }
        GameObject GameObject { get; }
    }
}