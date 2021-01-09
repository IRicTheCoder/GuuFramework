using Guu.Components.UI;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Guu.Patches.Core
{
    [HarmonyPatch(typeof (MainMenuUI))]
    [HarmonyPatch("Start")]
    internal static class MenuLoadPatch
    {
        public static void Postfix(MainMenuUI __instance)
        {
            SRGuu.OnMainMenuLoaded(__instance);

            foreach (TMP_Text text in Resources.FindObjectsOfTypeAll<TMP_Text>())
            {
                if (text.GetComponentInParent<TMP_Dropdown>() != null) continue;

                if (text.GetComponent<CustomLangSupport>() == null) text.gameObject.AddComponent<CustomLangSupport>();
            }
        }
    }
}