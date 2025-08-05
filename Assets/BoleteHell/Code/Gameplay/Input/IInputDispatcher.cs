using System;
using UnityEngine;

namespace BoleteHell.Code.Input
{
    public interface IInputDispatcher
    {
        bool IsMoving { get; }
        bool IsBoosting { get; }
        bool IsChargingShot { get; }
        bool IsDrawingShield { get; }
        bool IsDodging { get; }
        Vector2 MovementDisplacement { get; }
        Vector2 MousePosition { get; }
        Vector2 WorldMousePosition { get; }
        
        event Action OnShieldStart;
        event Action OnShieldEnd;
        event Action OnShoot;
        event Action<int> OnCycleWeapons;
        event Action<int> OnCycleShield;
    }
}