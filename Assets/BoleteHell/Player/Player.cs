using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public float movementSpeed { get; private set; } = 10;

    //TODO: ne devrait aps être des sfield, il va falloir les setter selon des upgrades ou des drop wtv
    [SerializeField]private List<LineSO> currentShields = new ();
    private int selectedShieldIndex;
    // [SerializeReference]
    // private Ray currentRay;
    [SerializeField]
    private List<Prism> equippedPrisms = new();
    private int selectedPrismIndex;

    [SerializeField] private Transform bulletSpawnPoint;
    
    
    

    //Pourrais être hardcodé pour que Q = refraction,E = reflexion, R = diffraction
    public void CycleShields(bool forward)
    {
        if (currentShields.Count <= 1)
        {
            Debug.LogWarning("No shields to cycle trough");
            return;
        }

        selectedShieldIndex = (selectedShieldIndex + (forward ? 1 : -1) + currentShields.Count) % currentShields.Count;

        Debug.Log($"selected {GetSelectedShield().name}");
        //TODO: trigger le changement du ui
    }

    public void DrawShield(Vector3 nextPos)
    {
       GetSelectedShield().DrawShieldPreview(nextPos);
    }

    public void FinishShield()
    {
        GetSelectedShield().FinishLine();
    }

    private LineSO GetSelectedShield()
    {
        return currentShields[selectedShieldIndex];
    }

    public void Shoot(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - bulletSpawnPoint.position).normalized;
        equippedPrisms[selectedPrismIndex].Shoot(bulletSpawnPoint.position,direction);
    }
}
