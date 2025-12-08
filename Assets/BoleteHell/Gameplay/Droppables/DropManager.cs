using System;
using BoleteHell.Utils;
using BoleteHell.Utils.LogFilter;
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
            public float DropletDropRadius = 0.5f;
        }
        
        [Inject]
        private Config _config;
        
        [Inject]
        private IObjectInstantiator _instantiator;

        private static readonly LogCategory _logDrop = new("Drop", Color.cyan);
        
        public void Drop(GameObject dropSource, GameObject droplet, LootTable lootTable)
        {
            int dropCount = lootTable.GetDropCount();
            if (dropCount == 0)
                return;
            
            for (int i = 0; i < dropCount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * _config.DropletDropRadius;
                Vector3 position = dropSource.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                _instantiator.InstantiateWithInjection(droplet, position, dropSource.transform.rotation, null);
            }
            
            Scribe.Log(_logDrop, "{0} dropped {1} x{2}", dropSource.name, droplet.name, dropCount);
        }
    }
}