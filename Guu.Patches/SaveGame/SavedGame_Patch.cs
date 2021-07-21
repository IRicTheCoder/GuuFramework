using Eden.Patching.Harmony;
using Guu.SaveSystem;
using JetBrains.Annotations;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.Persist;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.SaveGame
{
    [EdenHarmony.Wrapper(typeof(SavedGame))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class SavedGame_Patch
    {
        private static void Load_Postfix(GameV12 p_gameState) => SaveHandler.Load(p_gameState);
        private static void LoadSummary_Postfix(string saveName, ref GameData.Summary @return) => SaveHandler.LoadSummary(saveName, @return);
        private static void Save_Prefix(GameV12 p_gameState) => SaveHandler.Save(p_gameState);
    }
}