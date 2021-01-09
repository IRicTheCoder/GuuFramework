using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Guu.SaveGame;
using HarmonyLib;

namespace Guu.Patches.SaveGame
{
    [HarmonyPatch(typeof (AutoSaveDirector))]
    [HarmonyPatch("SaveGame")]
    internal static class SaveGamePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            using (IEnumerator<CodeInstruction> code = instr.GetEnumerator())
            {
                while (code.MoveNext())
                {
                    CodeInstruction i = code.Current;
                    if (i == null) continue;
                    
                    if (i.opcode == OpCodes.Call && i.operand is MethodInfo info && info.Name == "GetNextFileName")
                    {
                        yield return i;

                        // Moves on line to create context
                        code.MoveNext();
                        yield return code.Current;
                        
                        // Sets up the arguments for the Method Call (based on Context)
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldloc_3);

                        // The method call
                        yield return new CodeInstruction(OpCodes.Call, 
                                                         AccessTools.Method(typeof(SaveGamePatch), "SaveModdedData"));
                    }
                    else
                        yield return i;
                }
            }
        }

        public static void SaveModdedData(AutoSaveDirector director, string nextfilename)
        {
            ModdedSaveHandler.LoadGame(director.StorageProvider as FileStorageProvider, nextfilename);
        }
    }
}