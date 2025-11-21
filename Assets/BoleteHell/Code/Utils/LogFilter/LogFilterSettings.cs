using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Utils.LogFilter
{
    [CreateAssetMenu(fileName = "LogFilterSettings", menuName = "Bolete Hell/Log Filter Settings")]
    public class LogFilterSettings : ScriptableObject
    {
        [ListDrawerSettings(IsReadOnly = true, DraggableItems = false)]
        public List<LogCategory> CategoryConfigs = new();
        
        [Button] [HorizontalGroup("Buttons", Width = 150)]
        public void EnableAllCategories()
        {
            foreach (var category in CategoryConfigs)
            {
                category.Enabled = true;
            }
        }
        
        [Button] [HorizontalGroup("Buttons", Width = 150)]
        public void ResetAllCategories()
        {
            foreach (var category in CategoryConfigs)
            {
                category.Enabled = false;
            }
        }
        
        private void OnEnable()
        {
            RebuildCache();
        }
        
        private void OnValidate()
        {
            RebuildCache();
        }
        
        private void RebuildCache()
        {
            List<LogCategory> categories = LogCategory.GetAllCategories();
            
            HashSet<string> newIds = categories.Select(c => c.Name).ToHashSet();
            CategoryConfigs.RemoveAll(c => !newIds.Contains(c.Name));
            
            HashSet<string> existingIds = CategoryConfigs.Select(c => c.Name).ToHashSet();
            var itemsToAdd = categories.Where(c => !existingIds.Contains(c.Name));
            CategoryConfigs.AddRange(itemsToAdd);

            foreach (LogCategory category in CategoryConfigs)
            {
                LogCategory.GetCategoriesDictionary()[category.Name] = category;
            }
        }
    }
}
