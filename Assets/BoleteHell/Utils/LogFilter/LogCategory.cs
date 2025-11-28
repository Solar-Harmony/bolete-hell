using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace BoleteHell.Utils.LogFilter
{
    [Serializable]
    public class LogCategory
    {
        private static readonly Dictionary<string, LogCategory> _categories = new();
        
        public string Name;
        public Color Color = new(1f, 1f, 1f, 1f);
        public bool Enabled = false;
        
        public LogCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("LogCategory name cannot be null or whitespace.", nameof(name));
            
            Name = name;
            _categories[name] = this;
        }

        public LogCategory(string name, Color color)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("LogCategory name cannot be null or whitespace.", nameof(name));

            Name = name;
            Color = color;
            _categories[name] = this;
        }
        
        [CanBeNull]
        public LogCategory GetCategory(string name)
        {
            _categories.TryGetValue(name, out LogCategory category);
            return category;
        }
        
        public static List<LogCategory> GetAllCategories()
        {
            return _categories.Values.ToList();
        }
        
        public static Dictionary<string, LogCategory> GetCategoriesDictionary()
        {
            return _categories;
        }
    }
}