using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.SpawnManager;
using Pathfinding;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Services
{
    public class AIController : MonoBehaviour
    {
        [Inject]
        private IEntityRegistry _entities;

        [Inject]
        private ISpawnService _spawner;

        [Inject]
        private Camera _camera;
        
        [SerializeField]
        private SpawnList _yanick;
        
        [SerializeField]
        private SpawnList _duhaime;
        
        private void Start()
        {
            Vector3 bottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            Vector3 topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            
            Vector2? spawnPosition1 = FindNearestNavigablePos(bottomLeft);
            Vector2? spawnPosition2 = FindNearestNavigablePos(topRight);
            Debug.Assert(spawnPosition1 != null);
            Debug.Assert(spawnPosition2 != null);
            
            _spawner.Spawn(_yanick, spawnPosition1.Value);
            _spawner.Spawn(_duhaime, spawnPosition2.Value);
        }

        private Vector2? FindNearestNavigablePos(Vector2 pos)
        {
            GameObject player = _entities.GetPlayer();
            Debug.Assert(player);
            Debug.Assert(AstarPath.active);
            Debug.Assert(_camera);

            NNInfo nearestNode = AstarPath.active.GetNearest(pos, NNConstraint.Default);
            
            if (nearestNode.node != null)
            {
                return nearestNode.position;
            }

            return null;
        }
    }
}