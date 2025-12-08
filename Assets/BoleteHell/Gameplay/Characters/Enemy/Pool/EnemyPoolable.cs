using BoleteHell.Code.Gameplay.Damage.Effects;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Enemy.Factory
{
    public class EnemyPoolable : MonoBehaviour, IPoolable<Vector3, int>, ICustomDestroy
    {
        [Inject]
        private Pool _pool;

        [Inject]
        private IStatusEffectService _statusEffects;

        public void OnSpawned(Vector3 position, int groupID)
        {
            transform.position = position;
            GetComponent<AIGroupComponent>().GroupID = groupID;
        }

        public void OnDespawned()
        {
            _statusEffects.ClearStatusEffects(gameObject);
        }

        public void ReturnToPool()
        {
            _pool.Despawn(this);
        }
        
        public class Pool : MonoPoolableMemoryPool<Vector3, int, EnemyPoolable>
        {
        }
    }
}