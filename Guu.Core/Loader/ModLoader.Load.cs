using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Eden.Core.Utils;
using Eden.Patching.Harmony;
using Guu.Logs;
using Guu.Utils;
using HarmonyLib;

namespace Guu.Loader
{
    /// <summary>
    /// The Mod Loader, this is the class that will load all mods that are using Guu's modding identification
    /// system. Access this to check if a mod is loaded and get info on any mod.
    /// </summary>
    public static partial class ModLoader
    {
        //+ CONSTANTS
        private const string ASSEMBLY_VERSION_TAG = "{assemblyVersion}";
        private const string EXAMPLE_MOD_ID = "<guu>";

        #pragma warning disable 618
        private const AllowedPatchType SAFE_PATCH = AllowedPatchType.PREFIX | AllowedPatchType.POSTFIX | AllowedPatchType.FINALIZER | AllowedPatchType.REVERSE | AllowedPatchType.FIELD |
                                                    AllowedPatchType.PROPERTY | AllowedPatchType.EVENT | AllowedPatchType.OPERATOR;
        #pragma warning restore 618
        
        //+ VARIABLES
        internal static LoadingState loadStep = LoadingState.NONE;
        private static readonly Dictionary<string, ModMain> MODS = new Dictionary<string, ModMain>();
        
        //+ PROPERTIES
        /// <summary>The current load step/state</summary>
        public static LoadingState CurrentStep => loadStep;

        //+ GATHER PROCESS
        // Gathers all the mods
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        internal static void GatherMods()
        {
            YamlFile<GuuMod>[] modFiles = YamlUtils.GetModFiles();
            SortedSet<GuuMod> loadOrder = new SortedSet<GuuMod>(new ModComparer());

            // Filter what is valid and invalid for load
            foreach (YamlFile<GuuMod> modFile in modFiles)
            {
                GuuMod mod = modFile.Read();

                // Check for mod ID
                if (mod.ID == null || mod.ID.Equals(string.Empty))
                {
                    GuuCore.LOGGER?.LogWarning($"Missing 'modID' key on file '{modFile.Info.FullName}' or the value is blank. Skipping mod!");
                    continue;
                }
                
                // Check if it is not example mod
                if (mod.ID.Equals(EXAMPLE_MOD_ID))
                {
                    GuuCore.LOGGER?.LogWarning($"Found example mod ID on file '{modFile.Info.FullName}', please change the ID of the mod. Skipping mod!");
                    continue;
                }
                
                // Check for assembly to load
                if (mod.AssemblyName == null || mod.AssemblyName.Equals(string.Empty))
                {
                    GuuCore.LOGGER?.LogWarning($"Missing 'assembly' key on file  {modFile.Info.FullName}' or the value is blank. Skipping mod!");
                    continue;
                }

                // Check for guu version to see if it can be loaded
                if (mod.GuuVersion != null && !mod.GuuVersion.Equals(string.Empty))
                {
                    if (!ValidateVersion(mod.GuuVersion, GuuCore.GUU_VERSION))
                    {
                        GuuCore.LOGGER?.LogWarning($"Guu version is outdated. Requires at least '{mod.GuuVersion}' but has '{GuuCore.GUU_VERSION}'. Skipping mod!");
                        continue;
                    }
                }

                mod.Info = modFile.Info;
                MODS.Add(mod.ID, null);
                loadOrder.Add(mod);
            }

            // Loads the mods and generates their mod information
            foreach (GuuMod mod in loadOrder)
            {
                string modid = mod.ID;
                string mainAssembly = mod.AssemblyName;
                string version = mod.Version ?? ASSEMBLY_VERSION_TAG;
                bool unsafeCheck = mod.IsUnsafe;
                HashSet<string> required = new HashSet<string>(mod.RequiredMods);
                
                EdenHarmony harmony = new EdenHarmony(modid);

                // Checks if all required mods are available
                foreach (string req in required)
                {
                    if (!MODS.ContainsKey(req))
                        throw new Exception($"Missing required mod '{req}' when loading '{modid}'");
                }
                
                // Checks and loads the main assembly
                FileInfo assembly = new FileInfo(Path.Combine(mod.Info.Directory.FullName, mainAssembly + GuuCore.DLL_EXTENSION));
                if (!assembly.Exists)
                    throw new Exception($"Cannot load the main assembly '{mainAssembly}' for mod '{modid}'");
                
                Assembly modAssembly = AssemblyUtils.LoadWithSymbols(assembly.FullName);
                harmony.LatePatchAll(modAssembly, !unsafeCheck ? AllowedPatchType.ALL : SAFE_PATCH);
                
                // Checks if version as {assemblyVersion} tag, and replaces it
                if (version.Equals(ASSEMBLY_VERSION_TAG))
                    mod.Version = modAssembly.GetRuntimeVersion();
                
                // Obtains the ModMain
                Type mainType = modAssembly.GetTypes().Single(t => t.IsSubclassOf(typeof(ModMain)));
                ModMain main = Activator.CreateInstance(mainType) as ModMain;

                main.Assembly = modAssembly;
                main.Mod = mod;
                main.Logger = new ModLogger(modid);
                main.HarmonyInst = harmony;
                
                MOD_CONTEXTS.Add(modAssembly, new ModContext(main.Mod, main));

                // Finalizes the load process
                MODS[modid] = main;
            }
            
            // Loads all modules and registers them
            foreach (string modid in MODS.Keys)
            {
                ModMain main = MODS[modid];
                List<IModLoad> moduleMains = new List<IModLoad> { main };

                foreach (ModModuleAttribute module in main.GetType().GetCustomAttributes<ModModuleAttribute>())
                {
                    string name = module.moduleName;
                    string depID = module.dependentID;
                    string depVer = module.dependentVersion;

                    // Checks if dependent is available, and if so if the version matches
                    if (!MODS.ContainsKey(depID)) continue;
                    if (!ValidateVersion(depVer, MODS[depID].Mod.Version)) continue;
                    
                    // Checks and loads the assembly
                    FileInfo assembly = new FileInfo(Path.Combine(main.Mod.Info.Directory.FullName, name + GuuCore.DLL_EXTENSION));
                    if (!assembly.Exists)
                        throw new Exception($"Trying to load module '{name}' for mod '{modid}', but it wasn't found!");
                    
                    Assembly moduleAssembly = AssemblyUtils.LoadWithSymbols(assembly.FullName);
                    main.HarmonyInst?.LatePatchAll(moduleAssembly, !main.Mod.IsUnsafe ? AllowedPatchType.ALL : SAFE_PATCH);
                    
                    // Obtains the ModuleMain
                    Type mainType = moduleAssembly.GetTypes().Single(t => t.IsSubclassOf(typeof(ModuleMain)));
                    ModuleMain moduleMain = Activator.CreateInstance(mainType) as ModuleMain;

                    moduleMain.Assembly = moduleAssembly;
                    moduleMain.Main = main;
                    
                    MOD_CONTEXTS.Add(moduleAssembly, MOD_CONTEXTS[main.Assembly]);
                    
                    // Finalizes the load process for this mod's modules
                    moduleMains.Add(moduleMain);
                }

                // Finalizes the load process for all modules
                main.Mod.Modules = moduleMains.ToArray();
            }
            
            // Executes all late patches
            foreach (ModMain main in MODS.Values)
                main.HarmonyInst?.ExecuteLatePatches();
            
            // Initializes the mods
            Init();
            
            // Preloads the mods
            PreLoad();
            
            // Triggers registration for all mods
            Register();
        }
        
        //+ LOAD PROCESS
        // Load Step - INIT
        private static void Init()
        {
            loadStep = LoadingState.INIT;

            // INFO: Everything that should run on INIT
            
            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    EnumInjector.InjectAll(module.Assembly);
                    IsLoadedInjector.InjectAll(module.Assembly);
                    
                    module.RunLoadStep();
                }
            }
            ContextProvider.ProvideContext(null);
        }

        // Load Step - PRE_LOAD
        private static void PreLoad()
        {
            loadStep = LoadingState.PRE_LOAD;
            
            // INFO: Everything that should run on PRE_LOAD
            
            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    module.RunLoadStep();
                }
            }

            ContextProvider.ProvideContext(null);
        }

        // Load Step - REGISTER
        private static void Register()
        {
            loadStep = LoadingState.REGISTER;

            // INFO: Everything that should run on REGISTER
            
            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    module.RunLoadStep();
                }
            }
            APIHandler.OnHandleRegistration();

            ContextProvider.ProvideContext(null);
        }
        
        // Load Step - LOAD
        internal static void Load()
        {
            loadStep = LoadingState.LOAD;
            
            // INFO: Everything that should run on LOAD

            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    module.RunLoadStep();
                }
            }

            ContextProvider.ProvideContext(null);
        }
        
        // Load Step - HANDLE
        internal static void Handle()
        {
            loadStep = LoadingState.HANDLE;
            
            // INFO: Everything that should run on HANDLE

            APIHandler.OnHandleItems();
            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    module.RunLoadStep();
                }
            }
            APIHandler.OnClearMemory();

            ContextProvider.ProvideContext(null);
        }
        
        // Load Step - POST_LOAD
        internal static void PostLoad()
        {
            loadStep = LoadingState.POST_LOAD;
            
            // INFO: Everything that should run on POST_LOAD

            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    module.RunLoadStep();
                }
            }

            ContextProvider.ProvideContext(null);
        }
        
        // Load Step - COMMS
        internal static void Comms()
        {
            loadStep = LoadingState.COMMS;
            
            // INFO: Everything that should run on COMMS

            foreach (ModMain mod in MODS.Values)
            {
                ContextProvider.ProvideContext(mod.Assembly);
                foreach (IModLoad module in mod.Mod.Modules)
                {
                    module.RunLoadStep();
                }
            }

            ContextProvider.ProvideContext(null);
        }
        
        //+ HELPERS
        // Validates the version
        [SuppressMessage("ReSharper", "InvertIf")]
        private static bool ValidateVersion(string versionA, string versionB)
        {
            IComparer<int> comp = Comparer<int>.Default;

            string[] verA = versionA.Split('.');
            string[] verB = versionB.Split('.');
            
            bool valid = true;
            
            // Test for Major Version
            if (verA.Length >= 1)
            {
                if (!verA[0].Equals("*"))
                {
                    int res = comp.Compare(int.Parse(verA[0]), int.Parse(verB[0]));
                    valid = res <= 0;
                }
            }
                    
            // Test for Minor Version
            if (verA.Length >= 2)
            {
                if (!verA[1].Equals("*"))
                {
                    int res = comp.Compare(int.Parse(verA[1]), int.Parse(verB[1]));
                    valid = res <= 0;
                }
            }
                    
            // Test for Build Number
            if (verA.Length >= 3)
            {
                if (!verA[2].Equals("*"))
                {
                    int res = comp.Compare(int.Parse(verA[2]), int.Parse(verB[2]));
                    valid = res <= 0;
                }
            }
                    
            // Test for Revision Number
            if (verA.Length >= 4)
            {
                if (!verA[3].Equals("*"))
                {
                    int res = comp.Compare(int.Parse(verA[3]), int.Parse(verB[3]));
                    valid = res <= 0;
                }
            }

            return valid;
        }
    }
}