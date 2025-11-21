using BoleteHell.Code.Gameplay.Characters;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Services
{
    public class SharedBlackboard : MonoBehaviour
    {
        [SerializeField]
        private RuntimeBlackboardAsset _blackboard;
        
        [Inject]
        private IEntityFinder _entityFinder;
        
        private void Awake()
        {
            Debug.Assert(_blackboard);
            var player = _entityFinder.GetPlayer().gameObject;
            _blackboard.Blackboard.Variables.Find(v => v.Name == "Player").ObjectValue = player;
        }
    }
}