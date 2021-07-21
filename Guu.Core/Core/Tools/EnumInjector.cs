using System;
using System.Reflection;
using Eden.Core.Utils;
using Eden.Patching.Fixers;
using Guu.Loader;
using JetBrains.Annotations;

namespace Guu
{
    /// <summary>
    /// Injects the custom enum values into their fields
    /// </summary>
    internal static class EnumInjector
    {
        internal static void InjectAll(Assembly modAssembly)
        {
            foreach (Type type in modAssembly.GetTypes())
            {
                EnumInjectAttribute inject = type.GetCustomAttribute<EnumInjectAttribute>();

                if (inject == null) continue;
                
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (!field.IsInitOnly || !field.FieldType.IsEnum || !field.IsStatic) continue;
                    if ((int) field.GetValue(null) != 0) continue;

                    object newValue = EnumFixer.LoadValue(field.FieldType, NameResolve(field.Name)) ?? 
                                      EnumFixer.AddValue(field.FieldType, NameResolve(field.Name));
                    
                    field.SetValue(null, newValue);
                }
            }
        }

        /// <summary>
        /// Injects a new value for an enum
        /// </summary>
        /// <param name="name">The name to inject</param>
        /// <param name="relevantAssembly">The assembly for the mod using this, to provide context</param>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <returns>The enum created, or null if creation wasn't possible</returns>
        public static T? Inject<T>(string name, Assembly relevantAssembly = null) where T : struct, Enum
        {
            if (ModLoader.loadStep != LoadingState.INIT)
                throw new Exception("Enums can only be inject during 'INIT' loading step");

            string resolved = NameResolve(name, relevantAssembly ?? AssemblyUtils.GetRelevant());
            return EnumFixer.LoadValue<T>(resolved) ?? EnumFixer.AddValue<T>(resolved);
        }

        /// <summary>
        /// Resolves the name to add the mod ID to the beginning
        /// </summary>
        /// <param name="name">The name to be resolved</param>
        /// <param name="relevantAssembly">The assembly for the mod using this, to provide context</param>
        /// <returns>The resolved name</returns>
        public static string NameResolve(string name, Assembly relevantAssembly = null)
        {
            if (relevantAssembly != null)
            {
                if (!ModLoader.MOD_CONTEXTS.ContainsKey(relevantAssembly)) return name;
                
                return $"{ModLoader.MOD_CONTEXTS[relevantAssembly].Mod.ID.ToLower()}:{name}";
            }
            
            return $"{ModLoader.Context.Mod.ID.ToLower()}:{name}";
        }
    }
}