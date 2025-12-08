using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BoleteHell.Gameplay.Droppables
{
    [Serializable]
    public class LootTableEntry
    {
        [Min(0)]
        public int DropCount;
        
        [Min(0f)]
        [SuffixLabel("%")]
        public float PercentageChance;
    }
    
    [Serializable]
    public class LootTable
    {
        [TableList]
        [ValidateInput("IsTableSum100", "The sum of chances must be exactly 100%")]
        public List<LootTableEntry> DropTable = new()
        {
            new LootTableEntry { DropCount = 0, PercentageChance = 40f },
            new LootTableEntry { DropCount = 1, PercentageChance = 60f }
        };

        private bool IsTableSum100(List<LootTableEntry> table)
        {
            if (table == null || table.Count == 0) return true;
            return Mathf.Approximately(table.Sum(entry => entry.PercentageChance), 100f);
        }

        /// <summary>
        /// Rolls the weighted table and returns a drop count.
        /// </summary>
        public int GetDropCount()
        {
            if (DropTable == null || DropTable.Count == 0)
                return 0;

            float roll = Random.Range(0f, 100f);
            float cumulative = 0f;
            
            foreach (var entry in DropTable)
            {
                cumulative += entry.PercentageChance;
                if (roll < cumulative)
                    return entry.DropCount;
            }

            // fallback for floating point inaccuracies, though it should be rare.
            return DropTable[^1].DropCount;
        }
    }
}
