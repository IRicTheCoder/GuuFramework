using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Eden.Core.Reflection;
using Guu.Logs;
using Guu.Utils;
using HarmonyLib;
using UnityEngine;

namespace Guu
{
    /// <summary>The core for Guu</summary>
    public static partial class GuuCore
    {
        //+ CONSTANTS
        // General Strings
        internal const string DLL_EXTENSION = ".dll";
        
        internal const string LAUNCHER_EXE_NAME = "GuuLauncher.exe";
        internal const string MODINFO_FILE = "modinfo.yaml";
        internal const string LOG_FILE = "Guu.log";
        internal const string UNITY_LOG = "Unity.log";
        internal const string REPORT_NAME = "CrashReport {dateTime}.report";

        internal const string LOG_SEPARATOR = "--------------------------------------------";
        internal const string LOG_MSEPARATOR = "============================================";

        // Assembly Names
        internal const string CORE_ASSEM = "Guu.Core";
        internal const string API_ASSEM = "Guu.API";
        internal const string SAVE_ASSEM = "Guu.SaveSystem";
        internal const string WORLD_ASSEM = "Guu.World";
        internal const string DEV_TOOLS_ASSEM = "Guu.DevTools";
        internal const string PATCHES_ASSEM = "Guu.Patches";
        internal const string SRML_BRIDGE_ASSEM = "Guu.SRMLBridge";

        internal static readonly HashSet<string> GUU_MODULES = new HashSet<string>
        {
            CORE_ASSEM,
            API_ASSEM,
            SAVE_ASSEM,
            WORLD_ASSEM,
            DEV_TOOLS_ASSEM,
            PATCHES_ASSEM,
            SRML_BRIDGE_ASSEM
        };

        // Third Party Assemblies
        internal const string SRML_ASSEM = "SRML";
        
        // Asset Packs
        internal const string GUI_PACK = "SystemGUI.pack";

        // Folders
        internal static readonly string GUU_FOLDER = Path.Combine(Application.dataPath, "../Guu");

        internal static readonly string ASSETS_FOLDER = Path.Combine(Application.dataPath, "../Guu/Framework/Assets");
        internal static readonly string ASSEM_FOLDER = Path.Combine(Application.dataPath, "../Guu/Framework/Libraries");
        internal static readonly string LAUNCHER_FOLDER = Path.Combine(Application.dataPath, "../Guu/Framework/Launcher");

        internal static readonly string BINDINGS_FOLDER = Path.Combine(Application.dataPath, "../Guu/Bindings");
        internal static readonly string LIBS_FOLDER = Path.Combine(Application.dataPath, "../Guu/Libraries");
        internal static readonly string MODS_FOLDER = Path.Combine(Application.dataPath, "../Guu/Mods");
        internal static readonly string CONFIGS_FOLDER = Path.Combine(Application.dataPath, "../Guu/Configs");
        internal static readonly string SAVES_FOLDER = Path.Combine(Application.dataPath, "../Guu/Saves");
        internal static readonly string REPORT_FOLDER = Path.Combine(Application.dataPath, "../Guu/Reports");
        
        internal static readonly HashSet<string> DYNAMIC_FOLDERS = new HashSet<string>
        {
            BINDINGS_FOLDER,
            LIBS_FOLDER,
            MODS_FOLDER,
            CONFIGS_FOLDER,
            SAVES_FOLDER,
            REPORT_FOLDER
        };
        
        // General Values
        internal static readonly ModLogger LOGGER = new ModLogger("Guu");

        // Auto Populated
        public static readonly string GUU_VERSION;
        public static readonly BuildType GUU_BUILD;

        public static readonly string GAME_VERSION;
        public static readonly string GAME_BUILD;
        public static readonly string GAME_REVISION;
        public static readonly string GAME_STORE;

        public static readonly string UNITY_VERSION;
        
        public static readonly bool DEBUG;
        public static readonly bool GUU_DEBUG;
        public static readonly bool FULL_TRACE;
    }
}