using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Gameplay.GameState;
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
            if (Bases.Count == 0)
            {
                distance = float.MaxValue;
                return null;
            }

            var closestBase = Bases
                .Select(b => new { Base = b, Distance = Vector2.Distance(b.Position, pos) })
                .OrderBy(b => b.Distance)
                .FirstOrDefault();
            
            if (closestBase == null)
            {
                distance = float.MaxValue;
                return null;
            }
            
            distance = closestBase.Distance;
            return closestBase.Base;
        }

        private List<Base> _cache;
    }
}