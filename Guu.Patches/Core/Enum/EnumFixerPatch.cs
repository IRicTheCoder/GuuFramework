using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace Guu.Patches.Core
{
    [HarmonyPatch]
    internal static class EnumFixerPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("System.Enum"), "GetCachedValuesAndNames");
        }
        
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code)
        {
            using (IEnumerator<CodeInstruction> enumerator = code.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    CodeInstruction inst = enumerator.Current;

                    if (inst?.operand is MethodInfo mi && mi.Name.Equals("Sort"))
                    {
                        List<Label> labels = inst.labels;
                        
                        yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = labels };
                        yield return new CodeInstruction(OpCodes.Ldloca, 1);
                        yield return new CodeInstruction(OpCodes.Ldloca, 2);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EnumFixerPatch), "CorrectEnum"));
                        
                        yield return inst;
                        /*enumerator.MoveNext();
                        inst = enumerator.Current;
                        List<Label> labels = inst?.labels;
                        
                        
                        yield return inst;*/
                    }
                    else
                        yield return inst;
                }
            }
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private static void CorrectEnum(object type, ref int[] oldValues, ref string[] oldNames)
        {
            Type enumType = type as Type;
            
            if (!EnumFixer.FIXED_DATA.ContainsKey(enumType)) return;

            EnumFixer.FixData data = EnumFixer.FIXED_DATA[enumType];
            
            Array.Resize(ref oldValues, oldValues.Length + data.Data.Count);
            Array.Resize(ref oldNames, oldNames.Length + data.Data.Count);
            
            data.Data.Values.CopyTo(oldValues, oldValues.Length);
            data.Data.Keys.CopyTo(oldNames, oldNames.Length);

            Array.Sort(oldValues, oldNames, Comparer<int>.Default);
        }
    }
}