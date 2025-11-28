using BoleteHell.Code.Input;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnergyComponent))]
    public class PlayerEnergyComponent : MonoBehaviour
    {
        [Inject]
        private IInputDispatcher _inputDispatcher;
        
        private EnergyComponent _energy;
        
        private void Awake()
        {
            _energy = GetComponent<EnergyComponent>();
        }
        
        private void Update()
        {
            if (!_inputDispatcher.IsDrawingShield)
            {
                _energy.Replenish(Time.deltaTime);
            }
        }
    }
}