using System.Linq;
using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Services
{
    public class Director : IDirector
    {
        [Inject(Id = "Player")]
        private ISceneObject _player;

        [Inject]
        private IBaseService _bases;

        [Inject]
        private IEntityFinder _entityFinder;
        
        //Pourrais être changer pour un findClosestAlly qui prendrait en compte les faction et chercherais dans tout les entité
        public ISceneObject FindClosestShroom(Character self)
        {
            return _entityFinder
                .GetAllEnemies()
                .Where(e => e != self)
                .ToList()
                .FindClosestTo(e => e.Position, self.Position, out float distance);
        }

        public ISceneObject FindTarget(Character self)
        {
            Base closestBase = _bases.GetClosestBase(self.Position, out float distanceToClosestBase);
            if (!closestBase)
                return _player;
            
            float distanceToPlayer = Vector2.Distance(self.Position, _player.Position);
            return distanceToPlayer < distanceToClosestBase ? _player : closestBase;
        }
    }
}