using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Eden.Core.Diagnostics;
using Guu.Console;
using Guu.Services.Windows;
using Guu.Windows;
using UnityEngine;

namespace Guu.Logs
{
    /// <summary>
    /// The internal loggers, all mod loggers get routed here
    /// </summary>
    internal static class InternalLogger
    {
        //+ CONSTANTS
        internal static readonly string UNITY_LOG_FILE = Path.Combine(Application.persistentDataPath, "Player.log");
        internal static readonly string NEW_UNITY_LOG_FILE = Path.Combine(GuuCore.GUU_FOLDER, GuuCore.UNITY_LOG);
        
        private static readonly ModLogger UNITY_LOGGER = new ModLogger("Unity");
        private static readonly LogFile GUU_LOG = new LogFile(LogFile.FILE_PATH);
        
        private static readonly string[] TO_IGNORE = {
            @"\[Culture=.*\]",
            @"Joystick Names:.*"
        };
        
        //+ VARIABLES
        internal static SystemWindow reportWindow = WindowManager.RegisterWindow(new ReportWindow());
        internal static Exception handledException;

        //+ INITIALIZATION
        internal static void Init()
        {
            Application.logMessageReceived += UnityLog;
            AppDomain.CurrentDomain.UnhandledException += HandleUncaughtException;

            GuuConsole.Init();
        }

        //+ LOGGING
        internal static void Log(string message)
        {
            if (message.MatchesAny(TO_IGNORE)) return;
            
            string trace = string.Empty;
            if (GuuCore.FULL_TRACE && !message.Contains("-- FULL TRACE --")) trace = $"\n-- FULL TRACE --\n{new StackTrace()}";
                
            GuuConsole.Log(message + trace);
            GUU_LOG?.Log(message + trace);
        }
        
        internal static void UnityLog(string message, string trace, UnityEngine.LogType type)
        {
            if (string.IsNullOrWhiteSpace(message) || message.MatchesAny(TO_IGNORE))
                return;

            if (type == UnityEngine.LogType.Exception)
            {
                HandleException(new GuuUnmanagedException(message, trace), UNITY_LOGGER);
                return;
            }

            string toDisplay = message;
            if (!string.IsNullOrWhiteSpace(trace))
                toDisplay += "\n" + trace;

            if (type == UnityEngine.LogType.Error) toDisplay += GuuCore.GUU_DEBUG ? "\n-- FULL TRACE --\n" + new StackTrace() : string.Empty;

            UNITY_LOGGER?.Log(!message.Contains("WarningException") ? ModLogger.UNITY_TO_GUU[type] : ModLogger.UNITY_TO_GUU[UnityEngine.LogType.Warning], 
                             Regex.Replace(toDisplay, @"\[INFO]\s|\[ERROR]\s|\[WARNING]\s", ""));
        }

        //+ HANDLING
        private static void HandleUncaughtException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = args.ExceptionObject as Exception;
            HandleException(new GuuUnmanagedException("Caught Unhandled exception", new StackTrace().ToString(), ex));
        }
        
        internal static void HandleException(Exception exception, ModLogger logger = null, LogType type = LogType.CRITICAL)
        {
            if (handledException != null)
                return;

            string message = $"{(!exception.Message.Matches(@"^(?:(?!Exception:).)*Exception:(?!.*Exception:).*$") ? exception.GetType().Name + ": " : string.Empty)}{ReplaceInnerIfPresent(exception.Message, exception.InnerException)}";
            if (!string.IsNullOrWhiteSpace(exception.StackTrace)) message += $"\n{StackTracing.ParseStackTrace(new StackTrace(exception, true)).TrimEnd('\n')}";

            (logger ?? GuuCore.LOGGER)?.Log(type, message);
            
            File.Copy(UNITY_LOG_FILE, NEW_UNITY_LOG_FILE, true);

            if (type == LogType.CRITICAL)
            {
                handledException = exception;
                reportWindow.Open();
            }
        }

        /// <summary>
        /// Handles a thrown exception
        /// </summary>
        /// <param name="action">The action that could lead to the exception</param>
        /// <param name="logger">The logger to print the handled exception, null to use Guu's logger</param>
        public static void HandleThrow(Action action, ModLogger logger = null)
        {
            try { action.Invoke(); }
            catch (Exception e) { HandleException(e, logger); }
        }
        
        /// <summary>
        /// Handles a thrown exception
        /// </summary>
        /// <param name="func">The function that could lead to the exception</param>
        /// <param name="logger">The logger to print the handled exception, null to use Guu's logger</param>
        public static T HandleThrow<T>(Func<T> func, ModLogger logger = null)
        {
            try { return func.Invoke(); }
            catch (Exception e) { HandleException(e, logger); }

            return default;
        }
        
        //+ HELPERS
        internal static string ReplaceInnerIfPresent(string message, Exception inner) => inner == null ? message : message.ReplaceAll(new []
        {
            $"{inner.GetType().FullName}: {inner.Message}\n{inner.StackTrace}",
            $"{inner.GetType().Name}: {inner.Message}\n{inner.StackTrace}",
            $"{inner.Message}\n{inner.StackTrace}",
            $"{inner.GetType().FullName}: {inner.Message}",
            $"{inner.GetType().Name}: {inner.Message}",
            $"{inner.Message}",
            $"{inner.StackTrace}",
            "\n"
        }, string.Empty);
    }
}