namespace Guu.Logs
{
    /// <summary>
    /// The log type for the log system
    /// </summary>
    public enum LogType
    {
        /// <summary>A simple informative message.</summary>
        INFO,
        /// <summary>A warning message, something isn't as it is supposed to be.</summary>
        WARNING,
        /// <summary>An error message, something went wrong, but it is mostly a minor error.</summary>
        ERROR,
        /// <summary>A critical error message, triggered by exceptions, it is a relevant error as this should never happen, and may crash the game.</summary>
        CRITICAL
    }
}