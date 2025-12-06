using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Enemy.Factory
{
    public class EnemyPool
    {
        private readonly DiContainer _container;
        private readonly Dictionary<GameObject, EnemyPoolable.Pool> _pools = new();

        [Inject]
        public EnemyPool(DiContainer container)
        {
            _container = container;
        }

        public EnemyPoolable Spawn(GameObject prefab, Vector3 position, int groupID = -1)
        {
            var pool = GetOrCreatePool(prefab);
            return pool.Spawn(position, groupID);
        }

        private EnemyPoolable.Pool GetOrCreatePool(GameObject prefab)
        {
            if (_pools.TryGetValue(prefab, out var pool))
                return pool;
            
            var subContainer = _container.CreateSubContainer();
            subContainer.BindMemoryPool<EnemyPoolable, EnemyPoolable.Pool>()
                .WithInitialSize(0)
                .FromComponentInNewPrefab(prefab)
                .UnderTransformGroup($"EnemyPool_{prefab.name}");
            
            pool = subContainer.Resolve<EnemyPoolable.Pool>();
            _pools[prefab] = pool;
            
            return pool;
        }
    }
}