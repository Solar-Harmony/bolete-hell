using System;
using System.Diagnostics;
using UnityEngine;

namespace BoleteHell.Code.Utils.Editor.Logging.Logger
{
    [Serializable]
    public record LogEntry(
        LogType type,
        LogCategory category,
        DateTime timestamp,
        string formattedMessage,
        StackTrace stacktrace
    );
}