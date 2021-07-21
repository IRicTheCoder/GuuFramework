using System;
using System.Collections.Generic;
using System.Reflection;
using Eden.Core.Events;
using Guu.Console;
using Guu.Logs;
using ConsoleCommand = SRML.Console.ConsoleCommand;

namespace Guu.SRMLBridge
{
    /// <summary>A bridge used to interact with SRML Console</summary>
    public static class ConsoleBridge
    {
        //+ CONSTANTS
        internal static readonly HashSet<string> IGNORE_LIST = new HashSet<string>
        {
            "bind", "unbind", "debug", "addbutton", "removebutton", "editbutton",
            "clear", "help", "reload", ""
        };
        
        internal static readonly ModLogger SRML_LOGGER = new ModLogger("SRML");
        
        //+ VARIABLES
        internal static bool suppressLogs;
        
        //+ BRIDGE
        internal static void RegisterCommand(string cmdID, ConsoleCommand cmd)
        {
            if (IGNORE_LIST.Contains(cmdID)) return;
            if (GuuConsole.commands.ContainsKey(cmdID))
            {
                SRML_LOGGER?.Log($"Found command with conflicting id '{cmdID}', adding prefix, new id is 'srml.{cmdID}'");
                GuuConsole.RegisterCommand(new SRMLCommand(cmd, $"srml.{cmdID}"));
                return;
            }

            GuuConsole.RegisterCommand(new SRMLCommand(cmd));
        }
        
        internal static void PopulateCommands()
        {
            Dictionary<string, ConsoleCommand> cmds = 
                typeof(SRML.Console.Console).GetField("commands", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as Dictionary<string, ConsoleCommand>;
            if (cmds == null) return;
            
            foreach (string cmdID in cmds.Keys)
            {
                RegisterCommand(cmdID, cmds[cmdID]);
            }
        }

        internal static bool RunCatchers(string cmd, string[] args)
        {
            List<SRML.Console.Console.CommandCatcher> catchers = 
                typeof(SRML.Console.Console).GetField("catchers", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as List<SRML.Console.Console.CommandCatcher>;

            if (catchers == null || catchers.Count <= 0) return true;

            if (cmd.StartsWith("srml.")) cmd = cmd.Replace("srml.", "");
            
            bool keepExecution = true;
            foreach (SRML.Console.Console.CommandCatcher catcher in catchers)
            {
                keepExecution = catcher.Invoke(cmd, args);

                if (!keepExecution)
                    break;
            }

            return keepExecution;
        }

        [EventPriority(int.MinValue)]
        internal static void ReloadMods()
        {
            typeof(SRML.Console.Console).GetMethod("ReloadMods", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }
    }
}