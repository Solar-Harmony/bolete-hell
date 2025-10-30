using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Gameplay.GameState;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Base
{
    public class BaseService : IBaseService
    {
        // lazy load only once
        public List<Base> Bases => _cache ??= new List<Base>(Object.FindObjectsByType<Base>(FindObjectsSortMode.None));

        [Inject]
        private IGameOutcomeService _outcome; 
        
        private List<Base> _cache;
        
        public void NotifyBaseDied(Base theBase)
        {
            Bases.Remove(theBase);
            
            if (Bases.Count == 0)
            {
                _outcome.TriggerDefeat("All your bases were destroyed");
            }
        }
        
        public Base GetClosestBase(Vector2 pos, out float distance)
        {
            return Bases.FindClosestTo(b => b.Position, pos, out distance);
        }

        public Base GetWeakestBase()
        {
            return Bases
                .OrderBy(b => b.Health.CurrentHealth)
                .FirstOrDefault();
        }
    }
}