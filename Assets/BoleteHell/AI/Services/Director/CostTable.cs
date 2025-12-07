using System;
using UnityEngine;

namespace BoleteHell.AI.Services
{
    [CreateAssetMenu(fileName = "AICostTable", menuName = "BoleteHell/AI/Cost Table", order = 1)]
    public class CostTable : ScriptableObject
    {
        public CostEntry[] Entries;

        public void OnValidate()
        {
            // sort most expensive first
            Array.Sort(Entries, (a, b) => b.Cost.CompareTo(a.Cost));
        }
    }
}