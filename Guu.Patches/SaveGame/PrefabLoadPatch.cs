namespace Guu.Patches.SaveGame
{
    /*[HarmonyPatch(typeof (ScenePrefabInstantiator))]
    [HarmonyPatch("InstantiateActor")]
    internal static class PrefabLoadPatch
    {
        private static void Postfix(long actorId, GameObject __result)
        {
            ModdedSaveHandler.PopulateData(__result, "ident:" + actorId);
        }
    }*/
}