using System.Collections.Generic;
using BoleteHell.Arsenals.Shields;
using BoleteHell.Gameplay.Character;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Input.Controllers
{
    public class ShieldInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        [Inject]
        private ShieldPreviewFactory _shieldPreviewFactory;
        
        [SerializeField] 
        private GameObject shieldPreviewPrefab;
        
        [SerializeField] 
        private List<ShieldData> currentShields = new();
        
        [Inject]
        private IEntityFinder _entityFinder;
        
        private int _selectedShieldIndex;
        private ShieldPreviewDrawer _currentShieldPreview;
        private Player _player;
        
        private void Awake()
        {
            Debug.Assert(shieldPreviewPrefab);
            _player = _entityFinder.GetPlayer();
        }

        private void Update()
        {
            if (input.IsDrawingShield) DrawShield(input.WorldMousePosition);
        }

        private void OnEnable()
        {
            input.OnShieldStart += StartShield;
            input.OnShieldEnd += FinishShield;
            input.OnCycleShield += CycleShields;
        }

        private void OnDisable()
        {
            input.OnShieldStart -= StartShield;
            input.OnShieldEnd -= FinishShield;
            input.OnCycleShield -= CycleShields;
        }

        //Pourrais être hardcodé pour que Q = refraction,E = reflexion, R = diffractio
        private void CycleShields(int value)
        {
            if (currentShields.Count <= 1)
            {
                Debug.LogWarning("No shields to cycle trough");
                return;
            }

            _selectedShieldIndex = (_selectedShieldIndex + value + currentShields.Count) % currentShields.Count;
        }

        private void StartShield()
        {
            _currentShieldPreview = _shieldPreviewFactory.Create(GetSelectedShield());
            _currentShieldPreview.OnEnergyValidationRequested += required => _player.Energy.currentEnergy >= required;
            _currentShieldPreview.OnEnergySpendRequested += amount => _player.Energy.Spend(amount);
        }

        private void DrawShield(Vector3 nextPos)
        {
           _currentShieldPreview.DrawPreview(nextPos);
        }

        private void FinishShield()
        {
            _currentShieldPreview.FinishLine();
        }

        private ShieldData GetSelectedShield()
        {
            if (currentShields.Count == 0)
            {
                Debug.LogWarning("No shields to cycle trough");
                return null;
            }
            
            return currentShields[_selectedShieldIndex];
        }
    }
}