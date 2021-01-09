using System;
using System.Collections.Generic;
using Guu.Utils;
using UnityEngine;

namespace Guu.SaveGame
{
    ///<summary>Contains all custom data for mods</summary>
    public partial class ModdedData
    {
        //+ CONVERSION FUNCTION STRUCT
        ///<summary>A conversion function for the modded data</summary>
        public readonly struct ConversionFunction
        {
            public readonly Func<string, object> load;
            public readonly Func<object, string> save;

            public ConversionFunction(Func<string, object> load, Func<object, string> save = null)
            {
                this.load = load;
                this.save = save;
            }
        }
        
        //+ UNITY TYPE HELPERS
        private static string FromVector(Vector4 vector) => $"{vector.x};{vector.y};{vector.z};{vector.w}";
        private static Vector4 ToVector(string value)
        {
            string[] args = value.Split(';');
            return new Vector4(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
        }

        //+ CONVERSION FUNCTION AIDS
        // Primitive Conversions
        public static readonly ConversionFunction BYTE = new ConversionFunction(value => byte.Parse(value));
        public static readonly ConversionFunction SBYTE = new ConversionFunction(value => sbyte.Parse(value));
        public static readonly ConversionFunction SHORT = new ConversionFunction(value => short.Parse(value));
        public static readonly ConversionFunction USHORT = new ConversionFunction(value => ushort.Parse(value));
        public static readonly ConversionFunction INT = new ConversionFunction(value => int.Parse(value));
        public static readonly ConversionFunction UINT = new ConversionFunction(value => uint.Parse(value));
        public static readonly ConversionFunction LONG = new ConversionFunction(value => long.Parse(value));
        public static readonly ConversionFunction ULONG = new ConversionFunction(value => ulong.Parse(value));
        public static readonly ConversionFunction FLOAT = new ConversionFunction(value => float.Parse(value));
        public static readonly ConversionFunction DOUBLE = new ConversionFunction(value => double.Parse(value));
        public static readonly ConversionFunction DECIMAL = new ConversionFunction(value => decimal.Parse(value));
        public static readonly ConversionFunction BOOL = new ConversionFunction(value => bool.Parse(value));
        public static readonly ConversionFunction CHAR = new ConversionFunction(value => char.Parse(value));
        public static readonly ConversionFunction STRING = new ConversionFunction(null);
        
        // Unity Built-Ins
        public static readonly ConversionFunction VECTOR2 =
            new ConversionFunction(value => (Vector2) ToVector(value), value => FromVector((Vector2) value));
        
        public static readonly ConversionFunction VECTOR3 =
            new ConversionFunction(value => (Vector3) ToVector(value), value => FromVector((Vector3) value));
        
        public static readonly ConversionFunction VECTOR4 =
            new ConversionFunction(value => ToVector(value), value => FromVector((Vector4) value));

        public static readonly ConversionFunction COLOR =
            new ConversionFunction(value => ColorUtils.FromHex(value), value => ColorUtils.ToHexRGBA((Color) value));
        
        public static readonly ConversionFunction COLOR32 =
            new ConversionFunction(value => ColorUtils.FromHex(value), value => ColorUtils.ToHexRGBA((Color32) value));
    }
}