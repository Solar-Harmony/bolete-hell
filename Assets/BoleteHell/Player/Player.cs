using System.Collections.Generic;
using SerializeReferenceEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public float movementSpeed { get; private set; } = 10;

    //TODO: ne devrait aps être des sfield, il va falloir les setter selon des upgrades ou des drop wtv
    [SerializeField]private List<LineSO> currentShields = new List<LineSO>();
    [SerializeField]private int selectedShieldIndex;
    [SerializeReference]
    [SR(typeof(Ray))]
    private Ray currentRay;
    [SerializeField] private Transform bulletSpawnPoint;
    
    
    

    public void CycleShields(bool forward)
    {
        if (currentShields.Count <= 1)
        {
            Debug.LogWarning("No shields to cycle trough");
            return;
        }

        if (forward)
        {
            if (selectedShieldIndex + 1 == currentShields.Count)
                selectedShieldIndex = 0;
            else
                selectedShieldIndex++;
        }
        else
        {
            if (selectedShieldIndex - 1 < 0)
                selectedShieldIndex = currentShields.Count - 1;
            else
                selectedShieldIndex--;
        }
        
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

    public LineSO GetSelectedShield()
    {
        return currentShields[selectedShieldIndex];
    }

    public void Shoot(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - bulletSpawnPoint.position).normalized;
        currentRay.Cast(bulletSpawnPoint.position,direction);
    }
}
