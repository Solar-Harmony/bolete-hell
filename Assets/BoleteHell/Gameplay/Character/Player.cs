using BoleteHell.Gameplay.GameState;
using BoleteHell.Gameplay.Input;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Character
{
    public class Player : Character
    {
        [Inject]
        private IGameOutcomeService _outcome;
        
        [Inject]
        private IInputDispatcher _inputDispatcher;
        
        protected override void Awake()
        {
            base.Awake();
            Health.OnDeath += () =>
            {
                _outcome.TriggerDefeat("You have died");
            };
        }

        private void Update()
        {
            if (!_inputDispatcher.IsDrawingShield)
            {
                Energy?.Replenish(Time.deltaTime);
            }
        }
        
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + Health.CurrentHealth);
            GUI.Label(new Rect(10, 50, 300, 80), $"Energy: {Energy?.currentEnergy:F0} / {Energy?.maxEnergy}");
        }
    }
}