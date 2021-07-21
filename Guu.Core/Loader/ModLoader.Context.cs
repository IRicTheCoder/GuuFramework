using System.Collections.Generic;
using System.Reflection;
using Eden.Core.Code;
using Eden.Core.Context;
using Eden.Core.Utils;

namespace Guu.Loader
{
    /// <summary>
    /// The Mod Loader, this is the class that will load all mods that are using Guu's modding identification
    /// system. Access this to check if a mod is loaded and get info on any mod.
    /// </summary>
    public static partial class ModLoader
    {
        private static ModContext forcedContext;
        internal static readonly Dictionary<Assembly, ModContext> MOD_CONTEXTS = new Dictionary<Assembly, ModContext>();
        
        //+ PROPERTIES
        /// <summary>The context provider for events that require the mod as context</summary>
        // ReSharper disable once MemberCanBePrivate.Global
        internal static ModContextProvider ContextProvider => ModContextProvider.Instance;
        
        /// <summary>The current context being provided by the mod loader</summary>
        public static ModContext Context
        {
            get
            {
                if (forcedContext != null) return forcedContext;
                return ContextProvider.Context.As<ModContext>();
            }
        }

        /// <summary>A provider of context for mods</summary>
        public class ModContextProvider : Singleton<ModContextProvider>, IContextProvider
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            /// <summary>The context currently being provided</summary>
            public IContext Context { get; set; }

            /// <inheritdoc />
            public void ProvideContext(MethodInfo method, object target)
            {
                Assembly relevant = AssemblyUtils.GetRelevant(target);
                Context = MOD_CONTEXTS[relevant];
            }

            /// <summary>
            /// Facilitates the context being provided
            /// </summary>
            /// <param name="assembly">The assembly to provide context</param>
            public void ProvideContext(Assembly assembly)
            {
                if (assembly == null)
                {
                    Context = null;
                    return;
                }
                
                Context = MOD_CONTEXTS[assembly];
            }
        }
    }
}