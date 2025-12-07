using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.AI.Services
{
    [Serializable]
    public class CostEntry
    {
        [Required] [ValueDropdown(nameof(GetValidObjects))]
        public GameObject Prefab;
        
        public int Cost;

        private IEnumerable<ValueDropdownItem<GameObject>> GetValidObjects()
        {
            GameObject[] resources = Resources.LoadAll<GameObject>("EnemyArchetypes");
            
            foreach (GameObject go in resources)
            {
                yield return new ValueDropdownItem<GameObject>(go.name, go);
            }
        }
    }
}