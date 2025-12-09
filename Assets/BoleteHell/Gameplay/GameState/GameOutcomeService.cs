using System;
using System.Linq;
using BoleteHell.Code.Input;
using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.GameState
{
    public class GameOutcomeService : IGameOutcomeService
    {
        public event Action OnVictory;
        public event Action<string> OnDefeat;

        [Serializable]
        public class Config
        {
            public bool DefeatWhenAllBasesDestroyed = true;
        }
        
        [Inject]
        private IInputState _inputState;

        [Inject]
        private IEntityRegistry _entities;
        
        [Inject]
        private Config _config;

        [Inject]
        public void Construct()
        {
            _entities.EntityDied += ((EntityTag[] tags, GameObject obj) e) =>
            {
                if (e.tags.Contains(EntityTag.Player))
                    TriggerDefeat("You have been defeated.");
                else if (_config.DefeatWhenAllBasesDestroyed && e.tags.Contains(EntityTag.Base) && _entities.GetCount(EntityTag.Base) == 0)
                    TriggerDefeat("Your bases have been destroyed.");
            };
        }
        
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
            //_inputState.DisableInput();
        }
    }
}