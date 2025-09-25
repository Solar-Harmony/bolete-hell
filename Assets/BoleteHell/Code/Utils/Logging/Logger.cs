using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoleteHell.Code.Utils.Editor.Logging.Logger;
using UnityEngine;
using ILogger = BoleteHell.Code.Utils.Editor.Logging.Logger.ILogger;

namespace BoleteHell.Code.Utils.Editor.Logging
{
    public static class Log
    {
        private static readonly List<ILogger> Loggers = new();
        private const string Condition = "UNITY_EDITOR";
        
        public static void AddLogger(ILogger logger) => Loggers.Add(logger);
        public static void RemoveLogger(ILogger logger) => Loggers.Remove(logger);
        
#region LOG FUNCTIONS
        [Conditional(Condition)]
        public static void Debug(string message, params object[] args)
        {
            LogToAll(LogType.Log, null, message, args);
        }
        
        [Conditional(Condition)]
        public static void Debug(LogCategory category, string message, params object[] args)
        {
            LogToAll(LogType.Log, category, message, args);
        }
        
        [Conditional(Condition)]
        public static void Warning(string message, params object[] args)
        {
            LogToAll(LogType.Warning, null, message, args);
        }
        
        [Conditional(Condition)]
        public static void Error(string message, params object[] args)
        {
            LogToAll(LogType.Error, null, message, args);
        }
        
        [Conditional(Condition)]
        public static void Assert(string message, params object[] args)
        {
            LogToAll(LogType.Assert, null, message, args);
        }
#endregion

        private static void LogToAll(LogType type, LogCategory category, string message, params object[] args)
        {
            LogEntry entry = new(type, category, DateTime.Now, message, new StackTrace(1));
            foreach (ILogger logger in Loggers)
            {
                logger.Log(entry);
            }
        }
    }
}