using BoleteHell.Gameplay.Characters.Registry;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Services
{
    public class SharedBlackboard : MonoBehaviour
    {
        [SerializeField]
        private RuntimeBlackboardAsset _blackboard;
        
        [Inject]
        private IEntityRegistry _entityRegistry;
        
        private void Start()
        {
            Debug.Assert(_blackboard);
            var player = _entityRegistry.GetPlayer().gameObject;
            _blackboard.Blackboard.Variables.Find(v => v.Name == "Player").ObjectValue = player;
        }
    }
}