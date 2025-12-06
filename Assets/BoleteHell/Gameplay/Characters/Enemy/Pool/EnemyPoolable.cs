using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Enemy.Factory
{
    public class EnemyPoolable : MonoBehaviour, IPoolable<Vector3, int>, ICustomDestroy
    {
        [Inject]
        private Pool _pool;

        public void OnSpawned(Vector3 position, int groupID)
        {
            transform.position = position;
            GetComponent<AIGroupComponent>().GroupID = groupID;
        }

        public void OnDespawned()
        {
            
        }

        public void Destroy()
        {
            _pool.Despawn(this);
        }
        
        public class Pool : MonoPoolableMemoryPool<Vector3, int, EnemyPoolable>
        {
        }
    }
}