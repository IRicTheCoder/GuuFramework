using Eden.Patching.Harmony;
using Guu.SaveSystem;
using JetBrains.Annotations;
using MonomiPark.SlimeRancher;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.SaveGame
{
    [EdenHarmony.Wrapper(typeof(SavedProfile))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class SavedProfile_Patch
    {
        private static void LoadProfile_Postfix(SavedProfile @this) => SaveHandler.LoadProfile(@this.Profile);
        private static void LoadSettings_Postfix(SavedProfile @this) => SaveHandler.LoadSettings(@this.Settings);
        private static void SaveProfile_Prefix(SavedProfile @this) => SaveHandler.SaveProfile(@this.Profile);
        private static void SaveSettings_Prefix(SavedProfile @this) => SaveHandler.SaveSettings(@this.Settings);
    }
}