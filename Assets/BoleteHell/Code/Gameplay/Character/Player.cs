using BoleteHell.Code.Gameplay.GameState;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Character
{
    public class Player : Character
    {
        [Inject]
        private IGameOutcomeService _outcome;
        
        protected override void Awake()
        {
            base.Awake();
            health.OnDeath += () =>
            {
                _outcome.TriggerDefeat();
            };
        }
        
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + health.CurrentHealth);
        }
    }
}