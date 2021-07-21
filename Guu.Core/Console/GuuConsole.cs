using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Guu.Console.Commands;
using Guu.Logs;
using Guu.Windows;

namespace Guu.Console
{
    /// <summary>
    /// The console for Guu, controls everything 
    /// </summary>
    public static class GuuConsole
    {
        //+ CONSTANTS
        internal const string SUCCESS_COLOR = "#AAFF99";
        
        internal const int MAX_ENTRIES = 100;
        internal const int HISTORY = 10;

        //+ DELEGATES
        /// <summary>Represents dump action, that will dump information into a file</summary>
        public delegate void DumpAction(StreamWriter writer);

        /// <summary>Represents a command catcher, that triggers when a command is executed</summary>
        public delegate bool CommandCatcher(string cmd, string[] args, bool willExecute);
        
        //+ VARIABLES
        // General
        internal static bool updateConsole;
        
        // Functionality
        internal static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
        internal static Dictionary<string, ConsoleButton> buttons = new Dictionary<string, ConsoleButton>();
        internal static Dictionary<string, ConsoleButton> customButtons = new Dictionary<string, ConsoleButton>();

        internal static Dictionary<string, DumpAction> dumpActions = new Dictionary<string, DumpAction>();
        internal static HashSet<CommandCatcher> catchers = new HashSet<CommandCatcher>();
        
        // Interaction
        internal static List<string> lines = new List<string>();
        internal static List<string> history = new List<string>(HISTORY);

        internal static HashSet<string> commandUsage = new HashSet<string>();

        //+ INITIALIZATION
        internal static void Init()
        {
            // Register All Commands
            RegisterCommand(new PrintCommand());
            RegisterCommand(new AddButtonCommand());
            RegisterCommand(new RemoveButtonCommand());
            RegisterCommand(new EditButtonCommand());

            // Register Console Buttons
            RegisterButton(new ConsoleButton("clear", "Clear Console", "clear"));
            RegisterButton(new ConsoleButton("help", "Command List", "help"));
            RegisterButton(new ConsoleButton("mods", "Mods List", "mods"));
            RegisterButton(new ConsoleButton("reload", "Reload Mods", "reload"));
            RegisterButton(new ConsoleButton("dump", "Dump Info", "dump"));
            RegisterButton(new ConsoleButton("noclip", "No Clip", "noclip"));
            
            ConsoleBinder.ReadBinds();
        }
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new command to the console
        /// </summary>
        /// <param name="command">The command to register</param>
        /// <param name="replace">Set to true to replace the command if it already exists</param>
        /// <returns>True if registered, false otherwise</returns>
        public static bool RegisterCommand(ConsoleCommand command, bool replace = false)
        {
            string id = command.Command.ToLowerInvariant();
            
            if (commands.ContainsKey(id) && !replace)
            {
                (Loader.ModLoader.Context == null ? GuuCore.LOGGER : Loader.ModLoader.Context.Logger)
                    .LogWarning($"Failed to register command '<color=white>{id}</color>', it is already registered!");
                return false;
            }

            if (replace)
                commands[id] = command;
            else
                commands.Add(id, command);
            
            commandUsage.Add($"{id} {command.Arguments} - {command.Description}");
            return true;
        }

        /// <summary>
        /// Registers a new button to the console
        /// </summary>
        /// <param name="button">The button to register</param>
        /// <param name="replace">Set to true to replace the button if it already exists</param>
        /// <returns>True if registered, false otherwise</returns>
        public static bool RegisterButton(ConsoleButton button, bool replace = false)
        {
            string id = button.ID.ToLowerInvariant();

            if (id.Equals(string.Empty))
            {
                GuuCore.LOGGER.LogWarning($"Trying to register a button with an empty id, such if not possible!");
                return false;
            }
            
            if (id.Equals("all"))
            {
                (Loader.ModLoader.Context == null ? GuuCore.LOGGER : Loader.ModLoader.Context.Logger)
                    .LogWarning("Trying to register command button with id '<color=white>all</color>' but '<color=white>all</color>' is not a valid id!");
                return false;
            }
            
            if (button.Custom)
            {
                if (customButtons.ContainsKey(id) && !replace)
                {
                    GuuCore.LOGGER?.LogWarning($"Trying to register custom command button with id '<color=white>{id}</color>' but the ID is already registered!");
                    return false;
                }

                if (replace)
                {
                    if (!button.NoBinder) ConsoleBinder.EditBind(button);
                    customButtons[id] = button;
                }
                else
                {
                    if (!button.NoBinder) ConsoleBinder.RegisterBind(button);
                    customButtons.Add(id, button);
                }

                return true;
            }
            
            if (buttons.ContainsKey(id) && !replace)
            {
                (Loader.ModLoader.Context == null ? GuuCore.LOGGER : Loader.ModLoader.Context.Logger)
                    .LogWarning($"Trying to register command button with id '<color=white>{id}</color>' but the ID is already registered!");
                return false;
            }
            
            if (replace)
                buttons[id] = button;
            else
                buttons.Add(id, button);

            return true;
        }

        /// <summary>
        /// Registers a new dump action for the console
        /// </summary>
        /// <param name="id">The id for this dump action</param>
        /// <param name="action">The dump action to run</param>
        /// <param name="replace">Set to true to replace the dump action if it already exists</param>
        /// <returns>True if registered, false otherwise</returns>
        public static bool RegisterDumpAction(string id, DumpAction action, bool replace = false)
        {
            if (dumpActions.ContainsKey(id) && !replace)
            {
                (Loader.ModLoader.Context == null ? GuuCore.LOGGER : Loader.ModLoader.Context.Logger)
                    .LogWarning($"Failed to register command '<color=white>{id}</color>', it is already registered!");
                return false;
            }

            if (replace)
                dumpActions[id] = action;
            else
                dumpActions.Add(id, action);
            
            return true;
        }

        /// <summary>
        /// Registers a command catcher which allows commands to be processed and their execution controlled by outside methods
        /// </summary>
        /// <param name="catcher">The method to catch the commands</param>
        /// <returns>True if registered, false otherwise</returns>
        public static bool RegisterCatcher(CommandCatcher catcher) => catchers.Add(catcher);
        
        //+ MANIPULATION
        /// <summary>
        /// Removes a button from the console
        /// </summary>
        /// <param name="id">The id of the button to be removed</param>
        /// <param name="custom">Is it a custom button?</param>
        /// <returns>True if the button was removed, false otherwise</returns>
        public static bool RemoveButton(string id, bool custom = false)
        {
            if ((custom && !customButtons.ContainsKey(id)) || (!custom && !buttons.ContainsKey(id))) return false;

            if (custom)
            {
                ConsoleBinder.RemoveBind(id);
                customButtons.Remove(id);
            }
            else
                buttons.Remove(id);
            
            return true;
        }

        //+ INTERACTION
        // Logs a message into the console
        internal static void Log(string message)
        {
            if (lines.Count >= MAX_ENTRIES)
                lines.RemoveRange(0, 10);
            
            lines.Add(message);
            updateConsole = true;
        }

        // Processes the input from the console
        internal static void ExecuteCommand(string command, bool forced = false)
        {
            if (command.Equals(string.Empty))
                return;

            // If not forced adjusts the command history
            if (!forced)
            {
                if (history.Count == HISTORY) history.RemoveAt(0);
                if (history.Count == 0 || !history[history.Count - 1].Equals(command)) history.Add(command);
            }

            // Tries to process the command
            try
            {
                bool spaces = command.Contains(" ");
                string cmd = spaces ? command.Substring(0, command.IndexOf(' ')) : command;
                
                GuuCore.LOGGER?.Log($"<color=cyan>Command:</color> {command.Replace(cmd, $"<b>{cmd}</b>")}");
                
                // The command exists
                if (commands.ContainsKey(cmd))
                {
                    string[] args = spaces ? StripArgs(command) : null;
                    
                    // Runs through all the catchers before executing the command
                    bool keepExecution = InvokeCatchers(cmd, args, true);
                    if (!keepExecution) return;
                    
                    // Runs the command
                    ConsoleCommand consoleCommand = commands[cmd];
                    bool executed = consoleCommand.Execute(args);
                    
                    if (!executed) 
                        GuuCore.LOGGER?.Log($"<color=cyan>Usage:</color> <color=#77DDFF>{consoleCommand.Command} {ColorUsage(consoleCommand.Arguments)}</color>");
                }
                else
                    GuuCore.LOGGER?.LogError("Unknown command. Please use '<color=white>help</color>' for available commands.");
            }
            catch (Exception e)
            {
                // Handles any possible exception
                GuuCore.LOGGER?.LogError(e.Message + e.StackTrace);
            }
        }

        private static bool InvokeCatchers(string cmd, string[] args, bool keepExecution)
        {
            return catchers.Count <= 0 ? keepExecution : catchers.Aggregate(keepExecution, (current, catcher) => catcher.Invoke(cmd, args, current));
        }

        //+ HELPERS
        // Helps color the usage message
        internal static string ColorUsage(string usage)
        {
            string result = string.Empty;
            MatchCollection matches = Regex.Matches(usage, @"[\w\d]+|\<[\w]+\>|\[[\w]+\]");
            
            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("<") && match.Value.EndsWith(">"))
                {
                    result += $" <<color=white>{match.Value.Substring(1, match.Value.Length - 2)}</color>>";
                    continue;
                }

                if (match.Value.StartsWith("[") && match.Value.EndsWith("]"))
                {
                    result += $" <i>[<color=white>{match.Value.Substring(1, match.Value.Length - 2)}</color>]</i>";
                    continue;
                }

                result += " " + match.Value;
            }

            return result.TrimStart(' ');
        }
        
        // Helps strip the arguments from the command
        internal static string[] StripArgs(string command, bool autoComplete = false)
        {
            string argsPart = command.Substring(command.IndexOf(' ') + 1);
            MatchCollection result = Regex.Matches(argsPart, "[^'\"\\s\\n]+|'[^']+'?|\"[^\"]+\"?");
            List<string> args = new List<string>(result.Count);
            
            foreach (Match match in result)
                args.Add(autoComplete ? match.Value : Regex.Replace(match.Value, "'|\"", string.Empty));
            
            if (autoComplete && command.EndsWith(" "))
                args.Add(string.Empty);

            return args.ToArray();
        }
    }
}