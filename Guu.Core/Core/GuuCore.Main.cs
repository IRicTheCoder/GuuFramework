using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Eden.Core;
using Eden.Core.Diagnostics;
using Eden.Core.Utils;
using Eden.Patching.Harmony;
using Guu.Assets;
using Guu.Logs;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Guu
{
    /// <summary>The core for Guu</summary>
    public static partial class GuuCore
    {
        //+ VARIABLES
        // General
        private static bool isLoaded;
        internal static EdenHarmony harmony;
        internal static readonly FileInfo LAUNCHER_EXE;

        // Assemblies
        [UsedImplicitly] internal static Assembly core;
        [UsedImplicitly] internal static Assembly api;
        [UsedImplicitly] internal static Assembly save;
        [UsedImplicitly] internal static Assembly world;
        [UsedImplicitly] internal static Assembly devTools;
        [UsedImplicitly] internal static Assembly patches;

        [UsedImplicitly] internal static Assembly game;
        [UsedImplicitly] internal static Assembly srmlBridge;

        internal static readonly HashSet<Assembly> MAIN_ASSEMBLIES = new HashSet<Assembly>();
        internal static readonly HashSet<Assembly> ADDON_ASSEMBLIES = new HashSet<Assembly>();
        
        // Asset Packs
        internal static AssetPack guiPack;
        
        // App Environment
        internal static readonly HashSet<string> CMD_ARGS = new HashSet<string>();
        internal static readonly Process GAME_PROCESS;

        //+ STATIC INITIALIZER
        // Initializes everything Guu needs before injection
        static GuuCore()
        {
            // Grabs all command line arguments
            CMD_ARGS.Clear();
            foreach (string arg in Environment.GetCommandLineArgs()) CMD_ARGS.Add(arg);

            // Loads all Auto-Populated constants
            GUU_BUILD = Assembly.GetExecutingAssembly().GetBuildType();
            GUU_VERSION = Assembly.GetExecutingAssembly().GetRuntimeVersion();
            PopulateGameVersion(out GAME_VERSION, out GAME_BUILD, out GAME_REVISION, out GAME_STORE);
            UNITY_VERSION = Application.unityVersion;

            LAUNCHER_EXE = new FileInfo(Path.Combine(LAUNCHER_FOLDER, LAUNCHER_EXE_NAME));

            // Provides the Quit events
            Application.wantsToQuit += CanQuit;
            Application.quitting += Quit;

            // Generates internal and Debug information
            DEBUG = CMD_ARGS.Contains("--debug");
            GUU_DEBUG = CMD_ARGS.Contains("--guuDebug");
            FULL_TRACE = CMD_ARGS.Contains("--trace");

            GAME_PROCESS = Process.GetCurrentProcess();

            // Initializes global-level systems required by guu
            InternalLogger.Init();
        }

        //+ GUU INJECT
        // Loads the main system of Guu
        internal static void LoadGuu()
        {
            // Prevents the system from being loaded twice
            if (isLoaded)
            {
                LOGGER.Log(new StackTrace().ToString());
                return;
            }

            //& Removes Sentry SDK from the game
            SentrySdk sentrySdk = Object.FindObjectOfType<SentrySdk>();
            if (sentrySdk != null)
            {
                sentrySdk.Dsn = string.Empty;
                sentrySdk.GetType().GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
                sentrySdk.StopAllCoroutines();
                Application.logMessageReceived -= sentrySdk.OnLogMessageReceived;
                Object.Destroy(sentrySdk, 1);

                LOGGER.Log("Game is modded! Disabling Sentry SDK");
            }

            //& Run required system functions
            if (!CMD_ARGS.Contains("--guuLauncher") && !DEBUG) Application.Quit();

            // Generate the dynamic folders if they are not present
            foreach (string folder in DYNAMIC_FOLDERS)
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            }

            //& Starts Eden before everything else
            LogHandler.RegisterIfEmpty(LOGGER.Log, LOGGER.LogWarning, LOGGER.LogError, LOGGER.LogCritical);
            EdenUnity.Entry.EdenUnity.Init();
            
            // Add exceptions of types
            EdenHarmony.RegisterMethodException("SteamDLCProvider", "*");
            EdenHarmony.RegisterMethodException("EpicDLCProvider", "*");
            EdenHarmony.RegisterMethodException("DLCProvider", "*");
            EdenHarmony.RegisterMethodException("DLCDirector", "*");

            // Add exceptions of methods
            EdenHarmony.RegisterMethodException<GameContext>("Awake");
            
            // Add Name Resolve
            EdenHarmony.EnumNameResolve += EnumInjector.NameResolve;
            
            //& Starts the loading process and times it
            DateTime total = DateTime.Now;
            LOGGER.Log(LOG_MSEPARATOR);
            LOGGER.Log("GUU LOADING PROCESS STARTED");
            LOGGER.Log(LOG_SEPARATOR);

            //& Injects all assemblies, section is timed
            DateTime section = DateTime.Now;
            LOGGER.Log("[INJECTING ASSEMBLIES]");

            core = LoadAssembly(CORE_ASSEM);
            api = LoadAssembly(API_ASSEM);
            save = LoadAssembly(SAVE_ASSEM);
            world = LoadAssembly(WORLD_ASSEM);
            devTools = LoadAssembly(DEV_TOOLS_ASSEM);
            patches = LoadAssembly(PATCHES_ASSEM);

            game = Assembly.Load("Assembly-CSharp");

            // Load Bridge Assemblies
            if (ExceptionUtils.IgnoreErrors(() => Assembly.Load(SRML_ASSEM)) != null)
            {
                srmlBridge = LoadAssembly(SRML_BRIDGE_ASSEM, "Found SRML! ");
                Loader.ModLoader.srmlLoaderBridge = srmlBridge.ObtainTypeByForce("LoaderBridge");
            }

            LOGGER.Log($"[INJECTION COMPLETED] - {(DateTime.Now - section).TotalMilliseconds} ms");
            LOGGER.Log(LOG_SEPARATOR);

            //& Patches all assemblies, section is timed
            section = DateTime.Now;
            LOGGER.Log("[PATCHING ASSEMBLIES]");

            harmony = new EdenHarmony("Guu");
            if (patches != null) PatchAssembly(patches);
            if (srmlBridge != null) PatchAssembly(srmlBridge);

            LOGGER.Log($"[PATCHING COMPLETED] - {(DateTime.Now - section).TotalMilliseconds} ms");
            LOGGER.Log(LOG_SEPARATOR);

            //& Loads all asset packs, section is timed
            section = DateTime.Now;
            LOGGER.Log("[LOADING ASSET PACKS]");

            guiPack = LoadPack(GUI_PACK);

            LOGGER.Log($"[LOADING COMPLETED] - {(DateTime.Now - section).TotalMilliseconds} ms");
            LOGGER.Log(LOG_SEPARATOR);

            //& Loads all addon libraries
            section = DateTime.Now;
            LOGGER.Log("[LOADING ADDON LIBRARIES]");

            bool hasAddons = false;
            DirectoryInfo addons = new DirectoryInfo(LIBS_FOLDER);
            foreach (FileInfo file in addons.GetFiles($"*{DLL_EXTENSION}"))
            {
                hasAddons = true;

                ExceptionUtils.ThrowSuccessMessage(() =>
                {
                    Assembly loadedLib = AssemblyUtils.LoadWithSymbols(file.FullName);
                    ADDON_ASSEMBLIES.Add(loadedLib);

                    Type mainType = loadedLib.GetTypes().Single(t => t.IsSubclassOf(typeof(IAddonLoad)));

                    if (mainType == null) return;

                    IAddonLoad main = Activator.CreateInstance(mainType) as IAddonLoad;
                    main?.Initialize();
                }, $"- Loaded '{file.Name}'");
            }

            if (!hasAddons) LOGGER.Log("- No Addons were found");

            LOGGER.Log($"[LOADING COMPLETED] - {(DateTime.Now - section).TotalMilliseconds} ms");
            LOGGER.Log(LOG_SEPARATOR);
            
            //& Initializes all internal services 
            section = DateTime.Now;
            LOGGER.Log("[INITIALIZING INTERNAL SERVICES]");

            GuuServices.CreateServiceObject();
            GuuServices.InitInternalServices();

            LOGGER.Log($"[INITIALIZATION COMPLETED] - {(DateTime.Now - section).TotalMilliseconds} ms");
            LOGGER.Log(LOG_SEPARATOR);
            
            //& Finalize the loading process
            section = DateTime.Now;
            LOGGER.Log("[FINALIZING]");
            
            APIHandler.InitializeHandler();
            
            LOGGER.Log($"[FINALIZING COMPLETED] - {(DateTime.Now - section).TotalMilliseconds} ms");
            LOGGER.Log(LOG_SEPARATOR);
            LOGGER.Log($"SYSTEM IS FULLY OPERATIONAL - {(DateTime.Now - total).TotalMilliseconds} ms");
            LOGGER.Log(LOG_MSEPARATOR);

            // Marks loading completed
            isLoaded = true;

            // Finalizes
            LOGGER.Log("Starting the Mod Loader!");
            InternalLogger.HandleThrow(Loader.ModLoader.GatherMods);
        }

        //+ APPLICATION QUIT
        // Checks if the application has quit
        private static bool CanQuit() => InternalLogger.handledException == null;

        // Plays when the application quits
        private static void Quit()
        {
            File.Copy(InternalLogger.UNITY_LOG_FILE, InternalLogger.NEW_UNITY_LOG_FILE, true);
            if (!CMD_ARGS.Contains("--guuSilent") && !DEBUG && LAUNCHER_EXE.Exists) Process.Start(LAUNCHER_EXE.FullName);
        }
        
        //+ HELPERS
        // Loads an assembly
        private static Assembly LoadAssembly(string name, string prefix = null)
        {
            Assembly loaded = InternalLogger.HandleThrow(() => AssemblyUtils.LoadWithSymbols(Path.Combine(ASSEM_FOLDER, name + DLL_EXTENSION)));

            if (loaded != null)
            {
                LOGGER.Log($"- {prefix ?? string.Empty}Injected '{name}'");
                MAIN_ASSEMBLIES.Add(loaded);
            }
            else
                LOGGER.Log($"- Failed to Inject '{name}'");
            
            return loaded;
        }

        // Loads an Asset Pack
        private static AssetPack LoadPack(string name)
        {
            AssetPack pack = InternalLogger.HandleThrow(() => AssetLoader.LoadPack(Path.Combine(ASSETS_FOLDER, name)));

            LOGGER.Log($"- Loaded '{Path.GetFileNameWithoutExtension(name)}'");
            return pack;
        }

        // Patches an assembly
        private static void PatchAssembly(Assembly assembly, bool late = false, Type[] ignoreLate = null)
        {
            InternalLogger.HandleThrow(() =>
            {
                if (ignoreLate != null)
                {
                    foreach (Type type in ignoreLate)
                    {
                        harmony.PatchWrapper(type);
                    }
                }
                
                if (late)
                    harmony.LatePatchAll(assembly);
                else
                    harmony.PatchAll(assembly);
            });
            
            LOGGER.Log($"- Patched '{assembly.GetName().Name}'");
        }

        // Populates a game version
        private static void PopulateGameVersion(out string version, out string build, out string revision, out string store)
        {
            MessageDirector dir = GameContext.Instance?.MessageDirector;
            if (dir == null)
            {
                dir = Object.FindObjectOfType<MessageDirector>();
                dir.Awake();
            }

            MessageBundle bundle = dir.GetBundle(Bundles.BUILD_BUNDLE);
            DateTime buildRev = DateTime.ParseExact(bundle.Get("m.timestamp"), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            version = bundle.Get("m.version");
            build = $"{buildRev:ddMMyy}";
            revision = $"{buildRev:HHmmss}";
            
            store = TypeUtils.GetTypeBySearch("EpicDLCProvider") != null ? "Epic" : "DRM-Free";
            if (TypeUtils.GetTypeBySearch("SteamDLCProvider") != null) store = "Steam";
        }
    }
}