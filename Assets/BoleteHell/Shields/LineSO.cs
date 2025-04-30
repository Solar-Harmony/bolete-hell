using System;
using BulletHell.Scripts.Lines;
using SerializeReferenceEditor;
using Unity.VisualScripting;
using UnityEngine;

enum LineType
{
    Refract,
    Reflect,
    Disperse
}

[CreateAssetMenu(fileName= "LineSO",menuName = "LineData",order = 0)]
public class LineSO:ScriptableObject
{
    [SerializeField] private Color color;
    [SerializeField] private Sprite sprite;
    
    [SerializeReference] 
    [SR(typeof(LineHitLogic))]
    private LineHitLogic onRayHitLogic;
    
    [SerializeReference] 
    [SR(typeof(LineHitLogic))]
    private LineHitLogic onProjectileHitLogic;

    private GameObject shieldPreview;
    private LineDrawer lineDrawer;

   
    private void Setup()
    {
       shieldPreview = Resources.Load<GameObject>("ShieldPreview");
       if (!shieldPreview)
           Debug.LogError("Missing reference to ShieldPreview gameobject");

       GameObject obj = Instantiate(shieldPreview);
       lineDrawer = obj.GetComponent<LineDrawer>();
    }

    public void DrawShieldPreview(Vector3 nextPos)
    {
        if (!lineDrawer)
        {
            Setup();
        }

        lineDrawer.DrawPreview(nextPos);
    }

    public void FinishLine()
    {
        Debug.Log("Finishing line");
        lineDrawer.FinishLine(this);
    }

    public void OnRayHit(Vector3 incomingDirection,RaycastHit hitPoint,Ray ray)
    {
        onRayHitLogic.ExecuteRay(incomingDirection,hitPoint,ray);
        
    }

    public void OnProjectileHit(Vector3 incomingDirection)
    {
        onProjectileHitLogic.ExecuteProjectile(incomingDirection);
    }
    
    
}
