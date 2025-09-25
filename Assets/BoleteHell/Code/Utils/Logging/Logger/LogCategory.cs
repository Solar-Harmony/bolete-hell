using System.Collections.Generic;

namespace BoleteHell.Code.Utils.Editor.Logging
{
    public sealed class LogCategory
    { 
        public string Name { get; } 
            
        public LogCategory(string name)
        {
            Name = name;
            Register(this);
        }

        public static IEnumerable<LogCategory> GetAll() => Categories.Values;
        private static readonly Dictionary<string, LogCategory> Categories = new();
        private static void Register(LogCategory category)
        {
            Categories.Add(category.Name, category);
        }
    }
}