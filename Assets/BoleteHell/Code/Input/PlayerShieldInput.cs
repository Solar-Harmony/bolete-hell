using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Shields;
using UnityEngine;

namespace BoleteHell.Code.Input
{
    public class PlayerShieldInput : MonoBehaviour
    {
        [SerializeField] private InputController input;
        [SerializeField] private List<ShieldData> currentShields = new();
        private int _selectedShieldIndex;

        private void Update()
        {
            if (input.IsDrawingShield) DrawShield( input.WorldMousePosition);
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

            Debug.Log($"selected {GetSelectedShield().name}");
            //TODO: trigger le changement du ui
        }

        private void StartShield()
        {
            GetSelectedShield().StartLine();
        }

        private void DrawShield(Vector3 nextPos)
        {
            GetSelectedShield().DrawShieldPreview(nextPos);
        }

        private void FinishShield()
        {
            GetSelectedShield().FinishLine();
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