using UnityEngine;

namespace BoleteHell.Code.Utils.Editor.Logging.Logger
{
    public class UnityLogger : ILogger
    {
        public void Log(LogEntry entry)
        {
            string formatted = $"[{entry.category}] {entry.formattedMessage}\n{entry.stacktrace}";
            Debug.LogFormat(entry.type, LogOption.NoStacktrace, null, formatted);
        }
    }
}