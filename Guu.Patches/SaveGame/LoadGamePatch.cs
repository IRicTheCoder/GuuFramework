using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Guu.SaveGame;
using HarmonyLib;

namespace Guu.Patches.SaveGame
{
    [HarmonyPatch(typeof (AutoSaveDirector))]
    internal static class LoadGamePatch
    {
        private static readonly Type TARGET = typeof(AutoSaveDirector).GetNestedTypes(BindingFlags.NonPublic)
                                                                      .First(x => x.Name == "<LoadSave_Coroutine>d__68");

        public static MethodInfo TargetMethod()
        {
            return AccessTools.Method(TARGET, "MoveNext");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach (CodeInstruction i in instr)
            {
                if (i.opcode == OpCodes.Callvirt && i.operand is MethodInfo info && info.Name == "Load")
                {
                    yield return i;
                    
                    // First Argument for the Method Call
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, 
                                                     AccessTools.Field(TARGET, "<>4__this"));
                    
                    // Second Argument for the Method Call 
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, 
                                                     AccessTools.Field(TARGET, "saveName"));
                    
                    // The method call
                    yield return new CodeInstruction(OpCodes.Call, 
                                                     AccessTools.Method(typeof(LoadGamePatch), "LoadModdedData"));
                }
                else
                    yield return i;
            }
        }

        public static void LoadModdedData(AutoSaveDirector director, string saveName)
        {
            ModdedSaveHandler.LoadGame(director.StorageProvider as FileStorageProvider, saveName);
        }
    }
}