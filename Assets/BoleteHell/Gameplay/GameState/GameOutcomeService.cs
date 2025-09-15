using System;
using BoleteHell.Gameplay.Input;
using Zenject;

namespace BoleteHell.Gameplay.GameState
{
    public class GameOutcomeService : IGameOutcomeService
    {
        public event Action OnVictory;
        public event Action<string> OnDefeat;
        
        [Inject]
        private IInputState _inputState;
        
        public void TriggerVictory()
        {
            OnVictory?.Invoke();
        }

        public void TriggerDefeat(string reason)
        {
            _inputState.DisableInput();
            OnDefeat?.Invoke(reason);
        }
    }
}