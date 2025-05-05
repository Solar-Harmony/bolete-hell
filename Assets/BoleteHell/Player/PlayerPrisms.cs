using System;
using System.Collections.Generic;
using BulletHell.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPrisms : MonoBehaviour
{
    List<Prism> equippedPrisms = new();
    private int selectedPrismIndex;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private PlayerControls input;
    private InstantRayRenderer lineRenderer;
    private void OnEnable()
    {
        input.startedShooting.AddListener(OnShootStarted);
        input.endedShooting.AddListener(OnShootCanceled); 
        input.scrolled.AddListener(CycleWeapons);
    }

    private void OnDisable()
    {
        input.startedShooting.RemoveListener(OnShootStarted); 
        input.endedShooting.RemoveListener(OnShootCanceled); 
        input.scrolled.RemoveListener(CycleWeapons);

    }

    private void Update()
    {
        if (input.isShootingRay)
        {
            Shoot(input.mousePos2D);
        }
    }

    public void AddPrism(Prism prism)
    {
        equippedPrisms.Add(prism);
    }
    
    private void CycleWeapons(int value)
    {
        if (equippedPrisms.Count <= 1)
        {
            Debug.LogWarning("No weapons to cycle trough");
            return;
        }

        selectedPrismIndex = (selectedPrismIndex + value + equippedPrisms.Count) % equippedPrisms.Count;

        Debug.Log($"selected {GetSelectedWeapon().name}");
    }
    
    private Prism GetSelectedWeapon()
    {
        return equippedPrisms[selectedPrismIndex];
    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        //Reserve a line renderer
        Debug.Log("Started shooting");
        lineRenderer = LineRendererPool.Instance.GetRandomAvailable();
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        //Release the line renderer
        Debug.Log("finished shooting");

        LineRendererPool.Release(lineRenderer.Id);
    }

    private void Shoot(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - bulletSpawnPoint.position).normalized;
        equippedPrisms[selectedPrismIndex].Shoot(bulletSpawnPoint.position,direction,lineRenderer);
    }
}
