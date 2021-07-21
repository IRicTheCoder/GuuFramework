using System;
using System.Collections.Generic;
using Guu.Windows;
using UnityEngine;

namespace Guu.Services.Windows
{
    /// <summary>Used to register and manage system windows</summary>
    public static class WindowManager
    {
        //+ CONSTANTS
        internal const int WINDOW_MAX = 10;
        internal const int START_DEPTH = -short.MaxValue;
        
        //+ VARIABLES
        internal static readonly Dictionary<string, SystemWindow> WINDOWS = new Dictionary<string, SystemWindow>();

        //+ PROPERTIES
        /// <summary>The current skin of the system windows</summary>
        public static GUISkin Skin { get; internal set; }
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a system window
        /// </summary>
        /// <param name="window">The window to register</param>
        /// <returns>The window registered if registration was successful, null otherwise</returns>
        public static SystemWindow RegisterWindow(SystemWindow window)
        {
            if (WINDOWS.ContainsKey(window.ID))
            {
                GuuCore.LOGGER.LogWarning($"Trying to register system window with id '<color=white>{window.ID}</color>' but the ID is already registered!");
                return null;
            }
            
            WINDOWS.Add(window.ID, window);
            window.BuildWindow();
            return window;
        }
        
        //+ ACTIONS
        internal static void Open(string id)
        {
            if (!WINDOWS.ContainsKey(id)) throw new Exception($"Trying to open window '{id}' but it is not registered");
            if (WindowHandler.windowIDs.Contains(id))
            {
                GuuCore.LOGGER.LogWarning($"Tried to open a window that is already opened (ID: '{id}')");
                return;
            }
            if (WindowHandler.windowIDs.Count >= WINDOW_MAX)
            {
                GuuCore.LOGGER.LogWarning($"Tried to open a window but the max amount of '{WINDOW_MAX}' windows have been reached (ID: '{id}')");
                return;
            }

            WindowHandler.toOpen.Add(id);
        }

        internal static void Close(string id)
        {
            if (!WINDOWS.ContainsKey(id)) throw new Exception($"Trying to close window '{id}' but it is not registered");
            if (!WindowHandler.windowIDs.Contains(id))
            {
                GuuCore.LOGGER.LogWarning($"Tried to close a window that is already closed (ID: '{id}')");
                return;
            }
            if (WindowHandler.windowIDs.Count <= 0)
            {
                GuuCore.LOGGER.LogWarning($"Tried to close a window but there are no windows open (ID: '{id}')");
                return;
            }

            WindowHandler.toClose.Add(id);
        }
    }
}