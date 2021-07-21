using System.Collections.Generic;

namespace Guu.Console.Commands
{
    internal class EditButtonCommand : ConsoleCommand
    {
        //+ PROPERTIES
        public override string Command => "editbutton";
        public override string Arguments => "<id> <text> <command>";
        public override string Description => "Edits a user defined button from the command menu";
        public override string ExtDescription => 
            "<color=#77DDFF><id></color> - The id of the button. '<color=#77DDFF>all</color>' is not a valid id\n" +
            "<color=#77DDFF><text></color> - The text to display on the button\n" +
            "<color=#77DDFF><command></color> - The command the button will execute";

        //+ INTERACTIONS
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                GuuCore.LOGGER?.LogError($"The '{Command}' command takes 3 arguments");
                return false;
            }

            string id = args[0].Replace(" ", "_").ToLowerInvariant();
            string text = args[1].Equals("*") ? GuuConsole.customButtons[id].Text : args[1];
            string cmd = args[2].Equals("*") ? GuuConsole.customButtons[id].Command : args[2];
            
            if (args[0].Contains(" "))
                GuuCore.LOGGER?.LogWarning($"The '<id>' argument cannot contain any spaces. Using '_' instead! (New ID: {id})");
            
            bool result = GuuConsole.customButtons.ContainsKey(id) && GuuConsole.RegisterButton(new ConsoleButton(id, text, cmd, true), true);
            
            if (result)
                GuuCore.LOGGER?.Log($"Edited user defined button '{id}'", GuuConsole.SUCCESS_COLOR);

            return result;
        }

        public override HashSet<string> GetAutoComplete(int argIndex, string argText)
        {
            HashSet<string> args = null;
            
            if (argIndex == 0)
            {
                args = new HashSet<string>(GuuConsole.customButtons.Keys);
                FilterWords(args, argText);
            }
                
            if (argIndex == 2)
            {
                args = new HashSet<string>(GuuConsole.commands.Keys);
                FilterWords(args, argText);
            }

            return args;
        }
    }
}