using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BoleteHell.Code.Utils.LogFilter
{
    public static class LogCategoriesConstantsGenerator
    {
        [MenuItem("Tools/Generate Log Category Constants")]
        public static void Generate()
        {
            var settings = Resources.Load<LogFilterSettings>("LogFilterSettings");
            GenerateFromSettings(settings);
        }

#if UNITY_EDITOR
        private static void DelayedWriteAndRefresh(string path, string classCode, int constantCount)
        {
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                await File.WriteAllTextAsync(path, classCode);
                AssetDatabase.Refresh();
                Debug.Log($"LogCategories.cs generated with {constantCount} constants.");
            });
        }
#endif

        public static void GenerateFromSettings(LogFilterSettings settings)
        {
            if (!settings || settings.Categories == null)
            {
                Debug.LogError("LogFilterSettings or its categories are missing.");
                return;
            }

            if (settings.Categories.Count == 0)
                return;

            var lines = settings.Categories
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .Select(c => $"        public const string {Sanitize(c.Name)} = \"{c.Name}\";")
                .ToList();

            var classCode = "namespace BoleteHell.Code.Utils.LogFilter\n{\n    public static class LogCategories\n    {\n" + string.Join("\n", lines) + "\n    }\n}";
            
            var path = "Assets/BoleteHell/Code/Utils/LogFilter/LogCategories.cs";
            DelayedWriteAndRefresh(path, classCode, lines.Count);
        }

        private static string Sanitize(string name)
        {
            var valid = new string(name.Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(valid)) valid = "Category";
            return valid;
        }

#if UNITY_EDITOR
        private static bool _pending;
        static LogCategoriesConstantsGenerator()
        {
            EditorApplication.update += Update;
        }
        public static void RequestDeferredGeneration()
        {
            _pending = true;
        }
        private static void Update()
        {
            if (!_pending) return;
            _pending = false;
            var settings = Resources.Load<LogFilterSettings>("LogFilterSettings");
            GenerateFromSettings(settings);
        }
#endif
    }
}
