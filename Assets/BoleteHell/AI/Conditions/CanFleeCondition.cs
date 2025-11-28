using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(
        name: "ValidFleePositionExists", 
        story: "Can flee from [CurrentTarget] within [Distance] meters ➔ [FleePosition]", 
        category: "Bolete Hell", id: "c67ebb5f1f75b7d355bddabfa6ae34f4")]
    public partial class CanFleeCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<float> Distance;
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
        [SerializeReference] public BlackboardVariable<Vector2> FleePosition;
    
        public override bool IsTrue()
        {
            if (!CurrentTarget.Value)
                return false;
        
            LayerMask obstacleMask = LayerMask.GetMask("Obstacle"); 
            Vector3 selfPos = GameObject.transform.position;
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
                FleePosition.Value = fleeTargetPosition;
                break;
            }

            return validDirectionExists;
        }
    }
}
