using System;
using System.Collections.Generic;
using BulletHell.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShields : MonoBehaviour
{
    [SerializeField] private PlayerControls input;

    [SerializeField]private List<LineSO> currentShields ;
    private int selectedShieldIndex;

    private void OnEnable()
    {
        input.startedDrawingShield.AddListener(StartShield);
        input.endedDrawingShield.AddListener(FinishShield); 
        input.shieldCycled.AddListener(CycleShields);
    }

    private void OnDisable()
    {
        input.startedDrawingShield.RemoveListener(StartShield);
        input.endedDrawingShield.RemoveListener(FinishShield); 
        input.shieldCycled.RemoveListener(CycleShields);
    }

    private void Update()
    {
        if (input.isDrawingShield)
        {
            DrawShield(input.mousePos2D);
        }
    }

    //Pourrais être hardcodé pour que Q = refraction,E = reflexion, R = diffractio
    private void CycleShields(int value)
    {
        if (currentShields.Count <= 1)
        {
            Debug.LogWarning("No shields to cycle trough");
            return;
        }

        selectedShieldIndex = (selectedShieldIndex + value + currentShields.Count) % currentShields.Count;

        Debug.Log($"selected {GetSelectedShield().name}");
        //TODO: trigger le changement du ui
    }

    private void StartShield(InputAction.CallbackContext context)
    {
        GetSelectedShield().StartLine();
    }

    private void DrawShield(Vector3 nextPos)
    {
        GetSelectedShield().DrawShieldPreview(nextPos);
    }

    private void FinishShield(InputAction.CallbackContext context)
    {
        GetSelectedShield().FinishLine();
    }

    private LineSO GetSelectedShield()
    {
        return currentShields[selectedShieldIndex];
    }
}
