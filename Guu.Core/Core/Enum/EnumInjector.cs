using System;
using System.Reflection;

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
                    if (!field.IsInitOnly && !field.FieldType.IsEnum) continue;
                    if ((int) field.GetValue(null) != 0) continue;
                    
                    object newValue = EnumFixer.LoadValue(field.FieldType, field.Name) ?? 
                                      EnumFixer.AddValue(field.FieldType, field.Name);

                    field.SetValue(null, newValue);
                }
            }
        }
    }
}