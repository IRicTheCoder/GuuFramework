using Guu.SaveGame;
using HarmonyLib;

namespace Guu.Patches.SaveGame
{
    [HarmonyPatch(typeof (FileStorageProvider))]
    [HarmonyPatch("DeleteGameData")]
    internal static class DeleteGamePatch
    {
        private static void Postfix(FileStorageProvider __instance, string name)
        {
            ModdedSaveHandler.DeleteGame(__instance, name);
        }
    }
}