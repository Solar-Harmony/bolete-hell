using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BoleteHell.Gameplay.Droppables
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class DropRangeContext
    {
        public float dropChance = 60f;

        [SerializeField]
        public int min = 1;
    
        [SerializeField]
        public int max = 1;
    
        [SerializeField]
        public AnimationCurve dropCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Retourne un nombre entre min et max selon la curve
        /// </summary>
        /// <returns></returns>
        public int GetValueInRange()
        {
            float t = Random.value; 
            float curvedT = dropCurve.Evaluate(t); 
            float value = Mathf.Lerp(min, max, curvedT);
            return (int)value;
        }
    }
}
