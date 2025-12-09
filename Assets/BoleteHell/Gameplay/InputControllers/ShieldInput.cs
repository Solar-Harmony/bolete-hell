using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Shields;
using BoleteHell.Code.Input;
using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.InputControllers
{
    public class ShieldInput : MonoBehaviour
    {
        [Inject] 
        private IInputDispatcher input;
        
        [Inject]
        private ShieldPreviewFactory _shieldPreviewFactory;

        [Inject]
        private IEntityRegistry _entities;
        
        [SerializeField] 
        private GameObject shieldPreviewPrefab;
        
        [SerializeField] 
        private List<ShieldData> currentShields = new();
        
        private int _selectedShieldIndex;

        private ShieldPreviewDrawer _currentShieldPreview;

        private GameObject _player;

        private void Start()
        {
            Debug.Assert(shieldPreviewPrefab);
            _player = _entities.GetPlayer();
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
        public void CycleShields(int value)
        {
            if (currentShields.Count <= 1)
            {
                return;
            }

            _selectedShieldIndex = (_selectedShieldIndex + value + currentShields.Count) % currentShields.Count;
        }

        private void StartShield()
        {
            _currentShieldPreview = _shieldPreviewFactory.Create(_player.gameObject, GetSelectedShield());
        }

        private void DrawShield(Vector3 nextPos)
        {
           _currentShieldPreview.DrawPreview(nextPos);
        }

        private void FinishShield()
        {
            _currentShieldPreview.FinishLine();
        }

        public ShieldData GetSelectedShield()
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