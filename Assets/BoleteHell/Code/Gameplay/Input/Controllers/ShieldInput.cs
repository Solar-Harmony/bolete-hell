using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Shields;
using BoleteHell.Code.Gameplay.Character;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Input.Controllers
{
    public class ShieldInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        [Inject]
        private ShieldPreviewFactory _shieldPreviewFactory;

        [Inject]
        private IEntityFinder _entityFinder;
        
        [SerializeField] 
        private GameObject shieldPreviewPrefab;
        
        [SerializeField] 
        private List<ShieldData> currentShields = new();
        
        private int _selectedShieldIndex;

        private ShieldPreviewDrawer _currentShieldPreview;

        private Player _player;

        private void Start()
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
            _currentShieldPreview = _shieldPreviewFactory.Create(_player, GetSelectedShield());
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