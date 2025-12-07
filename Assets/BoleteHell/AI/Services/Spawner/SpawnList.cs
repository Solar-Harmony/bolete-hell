using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Gameplay.SpawnManager
{
    [CreateAssetMenu(menuName = "BoleteHell/AI/Spawn List")]
    public class SpawnList : ScriptableObject
    {
        [LabelText("List of enemies")]
        [ValueDropdown(nameof(GetValidObjects))]
        public GameObject[] allowedEnemies;

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
