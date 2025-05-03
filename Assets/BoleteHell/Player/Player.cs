using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public float movementSpeed { get; private set; } = 10;

    //TODO: ne devrait aps être des sfield, il va falloir les setter selon des upgrades ou des drop wtv
    [SerializeField]private List<LineSO> currentShields = new ();
    private int selectedShieldIndex;
    
    [SerializeField] private List<Prism> equippedPrisms = new();
    private int selectedPrismIndex;

    [SerializeField] private Transform bulletSpawnPoint;
    
    
    

    //Pourrais être hardcodé pour que Q = refraction,E = reflexion, R = diffraction
    public void CycleShields(int value)
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

    public void CycleWeapons(int value)
    {
        if (equippedPrisms.Count <= 1)
        {
            Debug.LogWarning("No weapons to cycle trough");
            return;
        }

        selectedPrismIndex = (selectedPrismIndex + value + equippedPrisms.Count) % equippedPrisms.Count;

        Debug.Log($"selected {GetSelectedWeapon().name}");
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
    private Prism GetSelectedWeapon()
    {
        return equippedPrisms[selectedPrismIndex];
    }

    public void Shoot(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - bulletSpawnPoint.position).normalized;
        equippedPrisms[selectedPrismIndex].Shoot(bulletSpawnPoint.position,direction);
    }
}
