using Eden.Patching.Harmony;
using Guu.Loader;
using Guu.Logs;
using SRML.Console;
using UnityEngine.SceneManagement;
using LogType = UnityEngine.LogType;

namespace Guu.SRMLBridge
{
    [EdenHarmony.Wrapper(typeof(SRML.Console.Console))]
    internal static class Console_Patch
    {
        private static void Init_Prefix()
        {
            ConsoleBridge.suppressLogs = true;
        }
        
        private static void Init_Postfix()
        {
            ConsoleBridge.suppressLogs = false;
            
            SceneManager.activeSceneChanged -= ConsoleWindow.AttachWindow;
            ConsoleBridge.PopulateCommands();
            ModLoader.Reload += ConsoleBridge.ReloadMods;
        }

        private static bool RegisterCommand_Prefix(ConsoleCommand cmd)
        {
            ConsoleBridge.RegisterCommand(cmd.ID, cmd);
            return false;
        }

        private static bool LogEntry_Prefix(LogType logType, string message)
        {
            if (ConsoleBridge.suppressLogs) return false;
            
            ConsoleBridge.SRML_LOGGER.Log(ModLogger.UNITY_TO_GUU[logType], message);
            return false;
        }

        private static bool AppLog_Prefix() => false;
    }
}