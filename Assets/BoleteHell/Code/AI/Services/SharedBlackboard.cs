using BoleteHell.Code.Gameplay.Characters;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Services
{
    public class SharedBlackboard : MonoBehaviour
    {
        [SerializeField]
        private BlackboardReference _blackboard;
        
        [Inject]
        private IEntityFinder _entityFinder;
        
        private void Awake()
        {
            Debug.Assert(_blackboard != null);
            
            _blackboard.SetVariableValue("Player", _entityFinder.GetPlayer().gameObject);
        }
    }
}