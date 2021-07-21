using System;

namespace Guu.Logs
{
    /// <summary>An exception not managed by guu</summary>
    internal class GuuUnmanagedException : Exception
    {
        public override string Message { get; }
        public override string StackTrace { get; }

        public GuuUnmanagedException(string message, string trace, Exception inner = null) : base(message, inner)
        {
            Message = message;
            StackTrace = trace;
        }
    }
}