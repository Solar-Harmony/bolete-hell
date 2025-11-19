using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BoleteHell.Code.Utils.LogFilter
{
    [Serializable]
    public class LogCategory
    {
        public string Name;
        public Color Color = new(1f, 1f, 1f, 1f);
        public bool Enabled = false;
    }
    
    public static class Scribe
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(LogType type, string categoryName, string message)
        {
            LogCategory category = LogFilterSettings.GetCategory(categoryName);
            if (!category.Enabled) 
                return;
            
            Color32 c = category.Color;
            string colorHex = $"#{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
            var format = $"<b><color={colorHex}>[{category.Name}]</color></b> {message}";
            Debug.LogFormat(type, LogOption.None, null, format);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string category, string message)
        {
            Log(LogType.Log, category, message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(string category, string message)
        {
            Log(LogType.Warning, category, message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(string category, string message)
        {
            Log(LogType.Error, category, message);
        }
    }
}