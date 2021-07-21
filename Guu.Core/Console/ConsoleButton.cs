namespace Guu.Console
{
    /// <summary>Represents a button for the console</summary>
    public class ConsoleButton
    {
        //+ VARIABLES
        internal string ID { get; }
        internal string Text { get; }
        internal string Command { get; }
        internal bool Custom { get; }
        internal bool NoBinder { get; }
        
        //+ CONSTRUCTOR
        internal ConsoleButton(string id, string text, string cmd, bool custom = false, bool noBinder = false)
        {
            ID = id;
            Text = text;
            Command = cmd.ToLowerInvariant();
            Custom = custom;
            NoBinder = noBinder;
        }
        
        //+ INTERACTIONS
        internal void Execute()
        {
            GuuConsole.ExecuteCommand(Command, true);
        }
        
        //+ OVERRIDES
        /// <inheritdoc/>
        public override string ToString() => $"{ID}:{Text}:{Command}";
    }
}