using Guu.Console;

namespace Guu.SRMLBridge
{
    // ReSharper disable once InconsistentNaming
    internal class SRMLCommand : ConsoleCommand
    {
        //+ PROPERTIES
        public override string Command { get; }
        public override string Arguments => Original.Usage.Replace($"{Command} ", "");
        public override string Description => Original.Description;
        public override string ExtDescription => Original.ExtendedDescription;
        public SRML.Console.ConsoleCommand Original { get; }

        //+ CONSTRUCTOR
        public SRMLCommand(SRML.Console.ConsoleCommand cmd, string id = null)
        {
            Command = string.IsNullOrWhiteSpace(id) ? cmd.ID.ToLowerInvariant() : id;
            Original = cmd;
        }

        //+ INTERACTION
        public override bool Execute(string[] args) => Original.Execute(args);
    }
}