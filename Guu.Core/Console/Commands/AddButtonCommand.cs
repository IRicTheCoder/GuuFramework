using System.Collections.Generic;

namespace Guu.Console.Commands
{
    internal class AddButtonCommand : ConsoleCommand
    {
        //+ PROPERTIES
        public override string Command => "addbutton";
        public override string Arguments => "<id> <text> <command>";
        public override string Description => "Adds a user defined button to the command menu";
        public override string ExtDescription => 
            "<color=#77DDFF><id></color> - The id of the button. '<color=#77DDFF>all</color>' is not a valid id\n" +
            "<color=#77DDFF><text></color> - The text to display on the button\n" +
            "<color=#77DDFF><command></color> - The command the button will execute";

        //+ INTERACTIONS
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                GuuCore.LOGGER?.LogError($"The '<color=white>{Command}</color>' command takes 3 arguments");
                return false;
            }

            string id = args[0].Replace(" ", "_").ToLowerInvariant();
            string text = args[1];
            string cmd = args[2];
            
            if (args[0].Contains(" "))
                GuuCore.LOGGER?.LogWarning($"The '<color=white><id></color>' argument cannot contain any spaces. Using '<color=white>_</color>' instead! (New ID: {id})");

            bool result = GuuConsole.RegisterButton(new ConsoleButton(id, text, cmd, true));
            
            if (result)
                GuuCore.LOGGER?.Log($"Added new user defined button '<color=white>{id}</color>' with command '<color=white>{cmd}</color>'", GuuConsole.SUCCESS_COLOR);

            return result;
        }

        public override HashSet<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex != 2) return null;
            
            HashSet<string> args = new HashSet<string>(GuuConsole.commands.Keys);
            FilterWords(args, argText);

            return args;
        }
    }
}