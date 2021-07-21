using System;
using System.Collections.Generic;
using System.Reflection;
using SRML;
using UnityEngine;

namespace Guu.SRMLBridge
{
    /// <summary>A bridge used to interact with SRML Mod Loader class</summary>
    public static class LoaderBridge
    {
        //+ CHECKS
        internal static bool IsModLoaded(string modid)
        {
            return SRModLoader.IsModPresent(modid);
        }

        // ReSharper disable once InconsistentNaming
        internal static bool IsSRMLAssembly(Assembly assembly)
        {
            Type loader = typeof(SRModLoader);
            MethodInfo method = loader.GetMethod("TryGetEntryType", BindingFlags.NonPublic | BindingFlags.Static);

            object result = method?.Invoke(null, new object[] { assembly, null });
            
            return (bool?) result ?? false;
        }
    }
}