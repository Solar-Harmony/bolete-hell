using System;

namespace BoleteHell.Code.Gameplay.GameState
{
    public class GameOutcomeService : IGameOutcomeService
    {
        public event Action OnVictory;
        public event Action OnDefeat;
        
        public void TriggerVictory()
        {
            OnVictory?.Invoke();
        }

        public void TriggerDefeat()
        {
            OnDefeat?.Invoke();
        }
    }
}