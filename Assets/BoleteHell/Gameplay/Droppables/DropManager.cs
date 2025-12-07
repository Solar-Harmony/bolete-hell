using System;
using System.Collections.Generic;
using BoleteHell.Utils;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BoleteHell.Gameplay.Droppables
{
    public class DropManager : IDropManager
    {
        [Serializable]
        public class Config
        {
            public List<GameObject> Droplets;
            public float DropletDropRadius = 0.5f;
        }
        
        [Inject]
        private Config _config;
        
        [Inject]
        private IObjectInstantiator _instantiator;
        
        public void TryDropLoot(GameObject dropSource, LootTable lootTable)
        {
            int dropCount = lootTable.GetDropCount();
            if (dropCount == 0)
                return;
            
            for (int i = 0; i < dropCount; i++)
            {
                int randomDropIndex = Random.Range(0, _config.Droplets.Count);
                
                Vector2 randomOffset = Random.insideUnitCircle * _config.DropletDropRadius;
                
                Vector3 position = dropSource.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                position.z = -1;
                
                _instantiator.InstantiateWithInjection(_config.Droplets[randomDropIndex], position, dropSource.transform.rotation, null);
            }
        }
    }
}