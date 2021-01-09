using System.Linq;
using System.Reflection;
using Guu.Utils;
using HarmonyLib;
using UnityEngine;

namespace Guu.Patches.Prefabs
{
    [HarmonyPatch(typeof(Object))]
    internal static class InstantiatePatch
    {
        public static MethodInfo TargetMethod()
        {
            return typeof(Object).GetMethods().First(x => x.Name == "Instantiate" && x.ContainsGenericParameters && x.GetParameters().Length == 1).MakeGenericMethod(typeof(Object));
        }

        public static void Postfix(Object __result)
        {
            if (__result is GameObject obj) FixInstantiation(obj);
        }

        internal static void FixInstantiation(GameObject obj)
        {
            if (obj.HasComponent<PrefabUtils.ModdedPrefab>())
                obj.SetActive(true);
        }
    }
    
    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("Instantiate",typeof(Object))]
    internal static class InstantiatePatch2
    {
        public static void Postfix(Object __result)
        {
            if (__result is GameObject obj) InstantiatePatch.FixInstantiation(obj);
        }
    }

    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("Instantiate", typeof(Object), typeof(Transform))]
    internal static class InstantiatePatch3
    {
        public static void Postfix(Object __result)
        {
            if (__result is GameObject obj) InstantiatePatch.FixInstantiation(obj);
        }
    }

    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("Instantiate", typeof(Object), typeof(Transform), typeof(bool))]
    internal static class InstantiatePatch4
    {
        public static void Postfix(Object __result)
        {
            if (__result is GameObject obj) InstantiatePatch.FixInstantiation(obj);
        }
    }

    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("Instantiate", typeof(Object), typeof(Vector3), typeof(Quaternion), typeof(Transform))]
    internal static class InstantiatePatch5
    {
        public static void Postfix(Object __result)
        {
            if (__result is GameObject obj) InstantiatePatch.FixInstantiation(obj);
        }
    }
    
    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("Instantiate", typeof(Object), typeof(Vector3), typeof(Quaternion))]
    internal static class InstantiatePatch6
    {
        public static void Postfix(Object __result)
        {
            if (__result is GameObject obj) InstantiatePatch.FixInstantiation(obj);
        }
    }
}