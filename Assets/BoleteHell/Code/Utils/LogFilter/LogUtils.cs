using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BoleteHell.Code.Utils.LogFilter
{
    public static class Scribe
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(LogType type, LogCategory cat, string message, params object[] args)
        {
            var category = LogCategory.GetCategoriesDictionary()[cat.Name];
            if (category is { Enabled: false }) 
                return;

            Color32 c = category.Color;
            string colorHex = $"#{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
            var format = $"<b><color={colorHex}>[{category.Name}]</color></b> {message}";
            Debug.LogFormat(type, LogOption.None, null, format, args);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(LogCategory category, string message, params object[] args)
        {
            Log(LogType.Log, category, message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(LogCategory category, string message, params object[] args)
        {
            Log(LogType.Warning, category, message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(LogCategory category, string message, params object[] args)
        {
            Log(LogType.Error, category, message, args);
        }
    }
}