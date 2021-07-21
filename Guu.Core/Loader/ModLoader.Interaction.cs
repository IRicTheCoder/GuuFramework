using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Eden.Core.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace Guu.Loader
{
    /// <summary>
    /// The Mod Loader, this is the class that will load all mods that are using Guu's modding identification
    /// system. Access this to check if a mod is loaded and get info on any mod.
    /// </summary>
    public static partial class ModLoader
    {
        //+ EVENTS
        /// <summary>Triggers when the Reload command is executed, use this to reload things in your mod. And adjust to possible
        /// reloaded configurations, among other things.</summary>
        public static event Action Reload;
        
        //+ VARIABLES
        internal static Type srmlLoaderBridge;

        //+ CHECKS
        /// <summary>
        /// Checks if a mod is loaded (To check for assemblies use <see cref="IsAssemblyLoaded"/>)
        /// </summary>
        /// <param name="modID">The mod id to check (use 'srml:' before the ID to check for SRML mods, use 'assem:' to
        /// check for any other kind of mods). When using 'assem:' the ID is the name of the assembly.</param>
        /// <returns>True if loaded, false otherwise</returns>
        [SuppressMessage("ReSharper", "InvertIf")]
        public static bool IsModLoaded(string modID)
        {
            if (modID.StartsWith("srml:"))
            {
                if (GuuCore.srmlBridge == null) return false;
                
                string trueID = modID.Replace("srml:", "");

                MethodInfo method = srmlLoaderBridge?.GetMethod("IsModLoaded", BindingFlags.NonPublic | BindingFlags.Static);
                object result = method?.Invoke(null, new object[] { trueID });

                return result != null && (bool) result;
            }

            if (modID.StartsWith("assem:"))
            {
                string trueID = modID.Replace("assem:", "");

                return IsAssemblyLoaded(trueID);
            }

            return MODS.ContainsKey(modID);
        }

        /// <summary>
        /// Checks if an assembly is loaded (To check for mods by ID use <see cref="IsModLoaded"/>)
        /// </summary>
        /// <param name="name">The name of the assembly</param>
        /// <returns>True if loaded, false otherwise</returns>
        public static bool IsAssemblyLoaded(string name)
        {
            return ExceptionUtils.IgnoreErrors(() => Assembly.Load(name)) != null;
        }

        //+ INTERACTIONS
        /// <summary>Gets the mod context for the given mod ID. Or null if none if found</summary>
        public static ModContext GetModContext(string modID) => MODS.ContainsKey(modID) ? GetModContext(MODS[modID].Assembly) : null;
        
        /// <summary>Gets the mod context for the given mod assembly. Or null if none if found</summary>
        public static ModContext GetModContext(Assembly modAssembly) => MOD_CONTEXTS.ContainsKey(modAssembly) ? MOD_CONTEXTS[modAssembly] : null;
        
        /// <summary>Gets the mod context for the given type (searching from it's assembly). Or null if none if found</summary>
        public static ModContext GetModContext(Type typeForContext) => MOD_CONTEXTS.ContainsKey(typeForContext.Assembly) ? MOD_CONTEXTS[typeForContext.Assembly] : null;

        /// <summary>Forces the context to the given mod ID. Always run <see cref="ClearForcedContext"/> once the context is no longer needed</summary>
        public static void ForceContext(string modID) => forcedContext = GetModContext(modID);

        /// <summary>Forces the context to the given mod assembly. Always run <see cref="ClearForcedContext"/> once the context is no longer needed</summary>
        public static void ForceContext(Assembly modAssembly) => forcedContext = GetModContext(modAssembly);

        /// <summary>Forces the context to the given given type (searching from it's assembly). Always run <see cref="ClearForcedContext"/> once the context is no longer needed</summary>
        public static void ForceContext(Type typeForContext) => forcedContext = GetModContext(typeForContext);

        /// <summary>Clears any forced mod context</summary>
        public static void ClearForcedContext() => forcedContext = null;
        
        // Reloads the mods
        internal static void ReloadMods() => Reload?.Handle(ContextProvider, true);

        //+ HELPERS
        // ReSharper disable once InconsistentNaming
        internal static bool IsSRMLAssembly(Assembly assembly)
        {
            if (GuuCore.srmlBridge == null) return false;
            
            MethodInfo method = srmlLoaderBridge?.GetMethod("IsSRMLAssembly", BindingFlags.NonPublic | BindingFlags.Static);
            object result = method?.Invoke(null, new object[] { assembly });
                        
            return result != null && (bool) result;
        }
        
        /// <summary>
        /// This is an helper method, it will provide a list with all unknown assemblies, it will filter
        /// any Game, Unity, System and Guu assemblies as all as any assembly loaded by Guu when loading
        /// mods and any assembly loaded by SRML if SRML is present.
        ///
        /// NOTE THAT in terms of SRML false positives might appear, some assemblies get loaded that are
        /// not considered mods, this means they will still appear on this list.
        ///
        /// However most results will, most likely be, UMF Mods or some other kind.
        ///
        /// The file can be found in the root of the game's folder
        /// </summary>
        public static void GetAllUnknownAssemblies()
        {
            HashSet<string> ignoreList = new HashSet<string>
            {
                "0Harmony", "INIFileParser", "Newtonsoft.Json", "discord-rpc", "InControlNative", "steam_api64",
                "XInputInterface64", "DOTween", "InControl", "InControl.Examples", "Logger", "mscorlib", "SRML",
                "SRML.Editor", "System", "UnityEngine", "HarmonySharedState", "uModFramework", "PluginManager.Core",
                "BouncyCastle.Crypto", "Ionic.Zip.Unity"
            };
            
            FileInfo dump = new FileInfo(Path.Combine(Application.dataPath, "../unknownAssemblies.txt"));

            using (StreamWriter writer = dump.CreateText())
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string name = assembly.GetName().Name;

                    if (name.StartsWith("Assembly-") || name.StartsWith("System.") || name.StartsWith("Unity.") ||
                        name.StartsWith("UnityEngine.") || name.StartsWith("Guu.") || name.StartsWith("Mono.") ||
                        name.StartsWith("uModFramework.") || name.StartsWith("Eden.") || name.StartsWith("EdenUnity."))
                        continue;
                    
                    if (MOD_CONTEXTS.ContainsKey(assembly)) continue;
                    if (GuuCore.ADDON_ASSEMBLIES.Contains(assembly)) continue;
                    if (ignoreList.Contains(name)) continue;
                    if (IsSRMLAssembly(assembly)) continue;

                    writer.WriteLine(name);
                }
                
                writer.Flush();
            }
        }
    }
}