using System;
using System.Reflection;

namespace Guu.Loader
{
    /// <summary>
    /// Injects the IsLoaded values into their fields
    /// </summary>
    internal static class IsLoadedInjector
    {
        internal static void InjectAll(Assembly modAssembly)
        {
            foreach (Type type in modAssembly.GetTypes())
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    IsLoadedAttribute inject = field.GetCustomAttribute<IsLoadedAttribute>();

                    if (inject == null) continue;
                    if (!field.IsInitOnly || !field.IsStatic) continue;
                    if (field.FieldType != typeof(bool)) continue;
                    
                    field.SetValue(null, Loader.ModLoader.IsModLoaded(inject.modID));
                }
            }
        }
    }
}