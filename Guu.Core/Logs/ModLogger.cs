using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Guu.Logs
{
    /// <summary>
    /// A logger for a mod
    /// </summary>
    public class ModLogger
    {
        //+ CONSTANTS
        internal static readonly Dictionary<UnityEngine.LogType, LogType> UNITY_TO_GUU = new Dictionary<UnityEngine.LogType, LogType>
        {
            { UnityEngine.LogType.Log, LogType.INFO },
            { UnityEngine.LogType.Assert, LogType.INFO },
            { UnityEngine.LogType.Warning, LogType.WARNING },
            { UnityEngine.LogType.Error, LogType.ERROR },
            { UnityEngine.LogType.Exception, LogType.CRITICAL }
        };

        // ReSharper disable StringLiteralTypo
        private static readonly Dictionary<LogType, string> TYPE_TO_TEXT = new Dictionary<LogType, string>
        {
            { LogType.INFO, "INFO" },
            { LogType.WARNING, "WARN" },
            { LogType.ERROR, "ERRO" },
            { LogType.CRITICAL, "CRIT" },
        };
        // ReSharper restore StringLiteralTypo

        private static readonly Dictionary<LogType, string> TYPE_TO_COLOR = new Dictionary<LogType, string>
        {
            { LogType.INFO, "white" },
            { LogType.WARNING, "#EEEE99" },
            { LogType.ERROR, "#FFC6AA" },
            { LogType.CRITICAL, "#FFAAAA" }
        };
        
        //+ VARIABLES
        private readonly string id;
        private LogFile file;
        
        //+ SETUP
        /// <summary>
        /// Creates a new logger
        /// </summary>
        /// <param name="id">ID of the mod using this logger</param>
        /// <param name="file">A custom file to log to when this logger is used</param>
        public ModLogger(string id, LogFile file = null)
        {
            this.id = id;
            this.file = file;
        }

        /// <summary>
        /// Sets a log file for this logger (replaces the one there if one is present)
        /// </summary>
        /// <param name="logFile">The log file</param>
        [UsedImplicitly]
        public void SetFile(LogFile logFile) => file = logFile;

        //+ LOGGING
        /// <summary>Logs an info message</summary>
        public void Log(string message) => Log(LogType.INFO, message);
        
        /// <summary>Logs an info message, with a specific color (color name or hex code with the #)</summary>
        public void Log(string message, string color) => Log(LogType.INFO, message, color);

        /// <summary>Logs a warning message</summary>
        public void LogWarning(string message) => Log(LogType.WARNING, message);

        /// <summary>Logs an error message</summary>
        public void LogError(string message) => Log(LogType.ERROR, message);

        /// <summary>Logs a critical error message</summary>
        public void LogCritical(string message) => Log(LogType.CRITICAL, message);

        /// <summary>
        /// Logs a message of a certain type
        /// </summary>
        /// <param name="type">Type to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="color">The color for this message</param>
        public void Log(LogType type, string message, string color = null)
        {
            if (message == null) return;
            
            string logType = TYPE_TO_TEXT[type];
            string textColor = color ?? TYPE_TO_COLOR[type];
            
            string fullMessage = $"<color=cyan>[{DateTime.Now:HH:mm:ss}]</color><color={textColor}>[{logType}][{id}] {message.RegexReplace(@"<material[^>]*>|<\/material>|<size[^>]*>|<\/size>|<quad[^>]*>|<b>|<\/b>", "").RegexReplace("((?:\"|').+(?:\"|'))", "<color=white>$1</color>")}</color>";
            
            InternalLogger.Log(fullMessage);
            file?.Log(fullMessage);
        }
    }
}