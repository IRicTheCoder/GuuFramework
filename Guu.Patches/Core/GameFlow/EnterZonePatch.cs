using HarmonyLib;

namespace Guu.Patches.Core
{
    [HarmonyPatch(typeof (PlayerState))]
    [HarmonyPatch("OnEnteredZone")]
    internal static class EnterZonePatch
    {
        private static void Postfix(ZoneDirector.Zone zone, PlayerState __instance)
        {
            SRGuu.OnZoneEntered(zone, __instance);
        }
    }
}