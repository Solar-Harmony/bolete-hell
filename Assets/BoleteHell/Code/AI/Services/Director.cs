using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Character;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Services
{
    public class Director : IDirector
    {
        [Inject(Id = "Player")]
        private ICharacter _player;

        [Inject]
        private IBaseService _bases;
        
        public ICharacter FindTarget(Character self)
        {
            Base closestBase = _bases.GetClosestBase(self.Position, out float distanceToClosestBase);
            if (!closestBase)
                return _player;
            
            float distanceToPlayer = Vector2.Distance(self.Position, _player.Position);
            return distanceToPlayer < distanceToClosestBase ? _player : closestBase;
        }
    }
}