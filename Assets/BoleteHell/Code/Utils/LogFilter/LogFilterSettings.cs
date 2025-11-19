using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Utils.LogFilter
{
    [CreateAssetMenu(fileName = "LogFilterSettings", menuName = "Bolete Hell/Log Filter Settings")]
    public class LogFilterSettings : ScriptableObject
    {
        private static Dictionary<string, LogCategory> _categoriesCache;
        
        public List<LogCategory> Categories = new();
        
        [Button] [HorizontalGroup("Buttons", Width = 150)]
        public void EnableAllCategories()
        {
            foreach (var category in Categories)
            {
                category.Enabled = true;
            }
        }
        
        [Button] [HorizontalGroup("Buttons", Width = 150)]
        public void ResetAllCategories()
        {
            foreach (var category in Categories)
            {
                category.Enabled = false;
            }
        }
        
        private bool _pendingCategoryConstantGeneration;
        private void OnValidate()
        {
            var validCategories = Categories.Where(c => !string.IsNullOrWhiteSpace(c.Name));
            _categoriesCache = validCategories.ToDictionary(c => c.Name, c => c);
#if UNITY_EDITOR
            LogCategoriesConstantsGenerator.RequestDeferredGeneration();
#endif
        }

        public static LogCategory GetCategory(string name)
        {
            return _categoriesCache[name];
        }
    }
}
