using System;
using System.Collections.Generic;

namespace Guu
{
    /// <summary>
    /// Fixes the enum values by generating new ones
    /// </summary>
    public static class EnumFixer
    {
        //+ FIX DATA CLASS
        /// <summary>Contains the data for the Fix applied</summary>
        internal class FixData
        {
            public Dictionary<string, int> Data { get; } = new Dictionary<string, int>();
        }
        
        //+ VARIABLES
        private static readonly Dictionary<Type, int> LAST_FREE_ID = new Dictionary<Type, int>();
        
        internal static readonly Dictionary<Type, FixData> FIXED_DATA = new Dictionary<Type, FixData>();
        
        //+ GENERATE ENUMS
        /// <summary>
        /// Adds a new value into an enum
        /// </summary>
        /// <param name="enumType">The type of enum</param>
        /// <param name="name">The name to identify the value</param>
        /// <returns>The resulting value</returns>
        public static object AddValue(Type enumType, string name)
        {
            if (!enumType.IsEnum) throw new Exception($"{enumType} is not a valid enum!");
            
            object free = FindFree(enumType);
            GenerateData(enumType, free, name);

            return free;
        }
        
        /// <summary>
        /// Adds a new value into an enum
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="name">The name to identify the value</param>
        /// <returns>The resulting value</returns>
        public static T AddValue<T>(string name)
        {
            return (T) AddValue(typeof(T), name);
        }

        /// <summary>
        /// Generates the fix data for a newly created enum value
        /// </summary>
        /// <param name="enumType">The type of enum</param>
        /// <param name="value">The value being added</param>
        /// <param name="name">The name to identify that value</param>
        private static void GenerateData(Type enumType, object value, string name)
        {
            if (!FIXED_DATA.ContainsKey(enumType))
                FIXED_DATA.Add(enumType, new FixData());
            
            FIXED_DATA[enumType].Data.Add(name, Convert.ToInt32(value));
        }
        
        /// <summary>
        /// Gets the first free space in the enum
        /// </summary>
        /// <param name="enumType">The type of enum</param>
        /// <returns>The value of that free space</returns>
        private static object FindFree(Type enumType)
        {
            object result;

            while (true)
            {
                if (!LAST_FREE_ID.ContainsKey(enumType))
                    LAST_FREE_ID.Add(enumType, 100000);
                else
                    LAST_FREE_ID[enumType]++;

                result = Enum.ToObject(enumType, LAST_FREE_ID[enumType]);

                if (!Enum.IsDefined(enumType, result)) break;
            }

            return result;
        }
        
        //+ SEARCH ENUM
        /// <summary>
        /// Loads an enum value added by the fixer
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <param name="name">The name that identifies the value</param>
        /// <returns>The value if found, null if not</returns>
        public static object LoadValue(Type enumType, string name)
        {
            if (!FIXED_DATA.ContainsKey(enumType) || !FIXED_DATA[enumType].Data.ContainsKey(name)) return null;

            return Enum.ToObject(enumType, FIXED_DATA[enumType].Data[name]);
        }
    }
}