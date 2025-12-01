using System;

namespace BoleteHell.Gameplay.GameState
{
    /// <summary>
    /// Invoke this service to trigger victory or defeat conditions in the game.
    /// Use the provided events to handle e.g. UI updates.
    /// </summary>
    public interface IGameOutcomeService
    {
        event Action OnVictory;
        event Action<string> OnDefeat;
        
        void TriggerVictory();
        void TriggerDefeat(string reason);
    }
}