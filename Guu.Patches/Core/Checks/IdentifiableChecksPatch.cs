using Guu.API;
using HarmonyLib;

namespace Guu.Patches.Core
{
    internal static class IdentifiableChecksPatch
    {
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsSlime")]
        internal static class IsSlimePatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsSlime(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsGordo")]
        internal static class IsGordoPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsGordo(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsLargo")]
        internal static class IsLargoPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsLargo(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsPlort")]
        internal static class IsPlortPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsPlort(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsAnimal")]
        internal static class IsAnimalPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsAnimal(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsChick")]
        internal static class IsChickPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsChick(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsFood")]
        internal static class IsFoodPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsFood(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsVeggie")]
        internal static class IsVeggiePatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsGordo(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsFruit")]
        internal static class IsFruitPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsGordo(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsCraft")]
        internal static class IsCraftPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsCraft(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsEcho")]
        internal static class IsEchoPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsEcho(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsEchoNote")]
        internal static class IsEchoNotePatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsEchoNote(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsOrnament")]
        internal static class IsOrnamentPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsOrnament(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsToy")]
        internal static class IsToyPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsToy(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsLiquid")]
        internal static class IsLiquidPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsLiquid(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsWater")]
        internal static class IsWaterPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsWater(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsFashion")]
        internal static class IsFashionPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsFashion(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsNonSlimeResource")]
        internal static class IsNonSlimeResourcePatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsNonSlimeResource(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsAllergyFree")]
        internal static class IsAllergyFreePatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsAllergyFree(id);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(Identifiable))]
        [HarmonyPatch("IsTarr")]
        internal static class IsTarrPatch
        {
            public static bool Prefix(Identifiable.Id id, out bool __result)
            {
                __result = IdentifiableHandler.IsTarr(id);
                return false;
            }
        }
    }
}