using System;
using BoleteHell.Gameplay.Characters;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class EliteObjective : MonoBehaviour, IObjective
{
    private HealthComponent _healthComponent;
    public event Action OnCompleted;

    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _healthComponent.OnDeath += () =>
        {
            OnCompleted?.Invoke();
        };
    }
    
    
    
    
}
