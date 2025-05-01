using System;
using BulletHell.Scripts.Lines;
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
   
    private LineHitLogic onHitLogic;

    private GameObject shieldPreview;
    private LineDrawer lineDrawer;

   //TODO: 
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
        //There has to be a better way than this to init
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

    public void OnRayHit(Vector3 incomingDirection,RaycastHit2D hitPoint,Ray ray)
    {
        onHitLogic.ExecuteRay(incomingDirection,hitPoint,ray);
        
    }

    public void OnProjectileHit(Vector3 incomingDirection)
    {
        onHitLogic.ExecuteProjectile(incomingDirection);
    }
    
    
}
