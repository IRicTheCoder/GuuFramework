using System.Collections.Generic;

namespace Guu.Console
{
    /// <summary>The base class for all console commands</summary>
    public abstract class ConsoleCommand
    {
        //+ VARIABLES
        /// <summary>The command you write in the console</summary>
        public abstract string Command { get; }

        /// <summary>The arguments info of this command (<X> for required arguments, [X] for optional arguments)</summary>
        public abstract string Arguments { get; }

        /// <summary>The summarized description of this command</summary>
        public abstract string Description { get; }

        /// <summary>The extended description for this command, displayed when inspecting the command (Multiline supported)</summary>
        public abstract string ExtDescription { get; }
        
        //+ INTERACTIONS
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="args">The arguments passed in the console (null if no arguments are provided)</param>
        /// <returns>True if it executed, false otherwise</returns>
        public abstract bool Execute(string[] args);
        
        /// <summary>
        /// Gets the auto complete list
        /// </summary>
        /// <param name="argIndex">The index of the argument in the command string</param>
        /// <param name="argText">The current text of the argument</param>
        /// <returns>The list of auto complete options</returns>
        public virtual HashSet<string> GetAutoComplete(int argIndex, string argText) => null;
        
        //+ HELPERS
        /// <summary>
        /// Filters the words from the hash set using a filter (normally the current argument text)
        /// </summary>
        /// <param name="list">The list to filter</param>
        /// <param name="filter">The filter text</param>
        public static void FilterWords(HashSet<string> list, string filter) => list.RemoveWhere(word => !word.StartsWith(filter) && !string.IsNullOrWhiteSpace(word));
    }
}