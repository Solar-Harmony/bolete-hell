using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ValidFleePositionExists", story: "Valid [fleePosition] exists within [distance] for [Self] away from [CurrentTarget]", category: "Bolete Hell", id: "c67ebb5f1f75b7d355bddabfa6ae34f4")]
public partial class ValidFleePositionExistsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
    [SerializeReference] public BlackboardVariable<Vector2> fleePosition;
    
    
    public override bool IsTrue()
    {
        LayerMask obstacleMask = LayerMask.GetMask("Obstacle"); 
        Vector3 selfPos = Self.Value.transform.position;
        Vector3 directionAway = (selfPos - CurrentTarget.Value.transform.position).normalized;
        
        List<Vector3> directions = new List<Vector3>
        {
            directionAway,
            Quaternion.Euler(0, 0, 30) * directionAway,
            Quaternion.Euler(0, 0, -30) * directionAway,
            Quaternion.Euler(0, 0, 60) * directionAway,
            Quaternion.Euler(0, 0, -60) * directionAway,
            Quaternion.Euler(0, 0, 90) * directionAway,
            Quaternion.Euler(0, 0, -90) * directionAway,

        };

        bool validDirectionExists = false;
        
        foreach (Vector3 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(selfPos, direction, Distance, obstacleMask);
            Debug.DrawRay(selfPos, direction * Distance, Color.green);
            
            if (hit) continue;
            validDirectionExists = true;
            Vector3 fleeTargetPosition = selfPos + direction * Distance;
            fleePosition.Value = fleeTargetPosition;
            break;
        }

        return validDirectionExists;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
