using System;
using BoleteHell.Code.Gameplay.SpawnManager;
using BoleteHell.Code.Input;
using Zenject;

namespace BoleteHell.Code.Gameplay.GameState
{
    public class GameOutcomeService : IGameOutcomeService
    {
        public event Action OnVictory;
        public event Action<string> OnDefeat;
        
        [Inject]
        private IInputState _inputState;

        [Inject]
        private SpawnController _spawnController;
        
        public void TriggerVictory()
        {
            OnVictory?.Invoke();
            EndGame();
        }

        public void TriggerDefeat(string reason)
        {
            OnDefeat?.Invoke(reason);
            EndGame();
        }

        private void EndGame()
        {
            _inputState.DisableInput();
            _spawnController.StopSpawning();
        }
    }
}