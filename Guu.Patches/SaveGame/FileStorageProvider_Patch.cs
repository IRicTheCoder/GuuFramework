using System.IO;
using Eden.Patching.Harmony;
using Guu.SaveSystem;
using JetBrains.Annotations;

namespace Guu.Patches.SaveGame
{
    [EdenHarmony.Wrapper(typeof(FileStorageProvider))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class FileStorageProvider_Patch
    {
        private static void DeleteGameData_Postfix(string name)
        {
            string realPath = Path.Combine(SaveHandler.SavePath, name + SaveHandler.GUU_EXTENSION);
            GuuCore.LOGGER.Log($"Attempting to delete modded save file {name}");

            if (!File.Exists(realPath))
            {
                GuuCore.LOGGER.Log("Modded save file wasn't found");
                return;
            }
            
            File.Delete(realPath);
        }

        private static void StoreGameData_Prefix(string name) => SaveHandler.SaveFile(name);
        private static void GetGameData_Postfix(string name) => SaveHandler.LoadFile(name);
    }
}