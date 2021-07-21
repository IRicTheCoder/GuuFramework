using System;
using System.Collections.Generic;
using Guu.Logs;

namespace Guu.Console.Commands
{
    internal class PrintCommand : ConsoleCommand
    {
        //+ PROPERTIES
        public override string Command => "print";
        public override string Arguments => "<text>";
        public override string Description => "Prints a text into the console";
        public override string ExtDescription => "<color=#77DDFF><text></color> - The text to print into the console\n";

        //+ INTERACTIONS
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                GuuCore.LOGGER?.LogError($"The '{Command}' command takes 1 argument");
                return false;
            }

            GuuCore.LOGGER.Log(args[0]);

            return true;
        }

        public override HashSet<string> GetAutoComplete(int argIndex, string argText) => null;
    }
}