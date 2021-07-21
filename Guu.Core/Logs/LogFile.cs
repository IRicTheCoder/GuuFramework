using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Guu.Logs
{
    /// <summary>
    /// Represents a Log file (.log)
    /// </summary>
    public class LogFile
    {
        //+ CONSTANTS
        internal static readonly string FILE_PATH = Path.Combine(GuuCore.GUU_FOLDER, GuuCore.LOG_FILE);
        
        //+ PROPERTIES
        public FileInfo Info { get; }
        
        //+ INITIALIZATION
        /// <summary>
        /// Creates a new Log file (overrides the old one if one exists)
        /// </summary>
        /// <param name="path">The path for said file</param>
        public LogFile(string path)
        {
            Info = new FileInfo(path);
            Info.Create().Close();
        }

        //+ LOGGING
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message to log</param>
        internal void Log(string message)
        {
            using (StreamWriter writer = Info.AppendText())
                writer.WriteLine($"{message.StripRichText()}");
        }
    }
}