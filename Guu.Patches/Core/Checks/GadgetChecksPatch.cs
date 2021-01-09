using Guu.API;
using HarmonyLib;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Core
{
    internal static class GadgetChecksPatch
    {
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsExtractor")]
        internal static class IsExtractorPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsExtractor(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsTeleporter")]
        internal static class IsTeleporterPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsTeleporter(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsWarpDepot")]
        internal static class IsWarpDepotPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsWarpDepot(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsMisc")]
        internal static class IsMiscPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsMisc(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsEchoNet")]
        internal static class IsEchoNetPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsEchoNet(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsDrone")]
        internal static class IsDronePatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsDrone(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsLamp")]
        internal static class IsLampPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsLamp(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsFashionPod")]
        internal static class IsFashionPodPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsFashionPod(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsSnare")]
        internal static class IsSnarePatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsSnare(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Gadget))]
        [HarmonyPatch("IsDeco")]
        internal static class IsDecoPatch
        {
            public static bool Prefix(Gadget.Id id, out bool __result)
            {
                __result = GadgetHandler.IsDeco(id);
                return false;
            }
        }
    }
}