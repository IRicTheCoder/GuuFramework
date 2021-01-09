using Guu.Utils;
using HarmonyLib;
using ModdedData = Guu.SaveSystem.ModdedData;

namespace Guu.Patches.Core
{
    [HarmonyPatch(typeof (GameContext))]
    [HarmonyPatch("Start")]
    internal static class GameContextPatch
    {
        private static void Postfix(GameContext __instance)
        {
            SRGuu.OnGameContextReady(__instance);
            
            GameUtils.AddComponentToIdentifiable<ModdedData>();
        }
    }
}