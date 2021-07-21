using System.Collections.Generic;

namespace Guu.Console.Commands
{
    internal class RemoveButtonCommand : ConsoleCommand
    {
        //+ PROPERTIES
        public override string Command => "removebutton";
        public override string Arguments => "<id>";
        public override string Description => "Removes a user defined button to the command menu";
        public override string ExtDescription => "<color=#77DDFF><id></color> - The id of the button. '<color=#77DDFF>all</color>' will remove all user generated buttons";

        //+ INTERACTIONS
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                GuuCore.LOGGER?.LogError($"The '<color=white>{Command}</color>' command takes 1 arguments");
                return false;
            }

            string id = args[0].Replace(" ", "_").ToLowerInvariant();

            if (args[0].Equals("all"))
            {
                foreach (string key in GuuConsole.customButtons.Keys)
                {
                    GuuConsole.RemoveButton(key);
                }

                GuuCore.LOGGER?.Log("Removed all user defined buttons", GuuConsole.SUCCESS_COLOR);
                return true;
            }
            
            if (args[0].Contains(" "))
                GuuCore.LOGGER?.LogWarning($"The '<color=white><id></color>' argument cannot contain any spaces. Using '<color=white>_</color>' instead! (New ID: {id})");

            bool result = GuuConsole.RemoveButton(id, true);
            
            if (result)
                GuuCore.LOGGER?.Log($"Removed the user defined button '<color=white>{id}</color>'", GuuConsole.SUCCESS_COLOR);

            return result;
        }

        public override HashSet<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex != 0) return null;
            
            HashSet<string> args = new HashSet<string>(GuuConsole.customButtons.Keys);
            FilterWords(args, argText);

            return args;
        }
    }
}