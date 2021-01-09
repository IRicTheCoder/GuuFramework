using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Guu
{
    /// <summary>
    /// The main class for Guu, used to make control the main things in the framework
    /// </summary>
    public partial class SRGuu
    {
        //+ ALL VARIABLES
        // Is the services initialized
        private static bool isInitialized = false;

        // The assemblies from Guu
        internal static Assembly mainAssembly;
        internal static Assembly apiAssembly;
        internal static Assembly devAssembly;

        // The harmony instance for Guu
        internal static Harmony harmony;

        // The object that controls the services update
        internal static GameObject serviceObj;

        //+ MAIN METHODS
        /// <summary>Enables the Guu services for your mod</summary>
        public static void Init()
        {
            if (isInitialized)
                return;

            // TODO: Fix this assembly mess, create an automated system for all Guu assemblies to be loaded.
            //? Alternatively, one could make all patches on the same class. 
            
            // Creates an Harmony instance and patches the game
            harmony = new Harmony("Guu");
            
            mainAssembly = Assembly.GetAssembly(typeof(SRGuu));
            harmony.PatchAll(mainAssembly);

            System.Type apiType = System.Type.GetType("RegistryItem");
            if (apiType != null)
            {
                apiAssembly = Assembly.GetAssembly(apiType);
                harmony.PatchAll(apiAssembly);
            }
            
            System.Type devType = System.Type.GetType("DebugHandler");
            if (devType != null)
            {
                devAssembly = Assembly.GetAssembly(devType);
                harmony.PatchAll(devAssembly);
            }

            // Marks as initialized
            isInitialized = true;
        }
    }
}