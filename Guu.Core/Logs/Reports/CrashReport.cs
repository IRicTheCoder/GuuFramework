using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Eden.Core.Diagnostics;
using Eden.Core.Utils;
using Guu.Loader;
using Guu.Utils;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Guu.Logs
{
    /// <summary>Represents a Crash Report to explain a certain crash</summary>
    internal class CrashReport
    {
        //+ CONSTANTS
        private const string VALUE_UNKOWN = "Unknown";
        private const int MAX_REPORTS = 10; 
        private static readonly string REPORT_FILE = Path.Combine(GuuCore.REPORT_FOLDER, GuuCore.REPORT_NAME);
        
        //+ VARIABLES
        internal string DisplayMessage { get; private set; }
        
        //+ CONSTRUCTOR
        internal CrashReport(Exception exception) => ExceptionUtils.ThrowMessage(() => BuildReport(exception), "{exception}");

        //+ BUILD
        private void BuildReport(Exception exception)
        {
            DisplayMessage = $"<b>Crash Report - {DateTime.Now:dd/MM/yyyy - HH:mm:ss}</b>\n";
            
            //& Generate machine specs
            //? Should have Device Model, Graphics Information
            DisplayMessage += "\n<b>[SYSTEM INFO]</b>\n";
            DisplayMessage += $"<b>Device Model:</b> {SystemInfo.deviceModel}  <b>Identifier:</b> {SystemInfo.deviceUniqueIdentifier}\n";
            DisplayMessage += $"<b>OS:</b> {SystemInfo.operatingSystem}  <b>Family:</b> {SystemInfo.operatingSystemFamily}  <b>64 Bits:</b> {Environment.Is64BitOperatingSystem.ToHumanString()}\n";
            DisplayMessage += $"<b>CPU:</b> {SystemInfo.processorType}  <b>Frequency:</b> {Mathf.Ceil(SystemInfo.processorFrequency/1024f):N2} GHz  <b>Threads:</b> {SystemInfo.processorCount}\n";
            DisplayMessage += $"<b>GPU:</b> {SystemInfo.graphicsDeviceName}  <b>Memory:</b> {Mathf.Ceil(SystemInfo.graphicsMemorySize/1024f):N2} GB  <b>Type:</b> {SystemInfo.graphicsDeviceType}  <b>Driver:</b> {SystemInfo.graphicsDeviceVersion}\n";
            DisplayMessage += $"<b>RAM:</b> {Mathf.CeilToInt(Profiler.GetTotalReservedMemoryLong()/1048576f):N2} MB / {SystemInfo.systemMemorySize:N2} MB\n";
            
            //& Generate game information
            //? Should have version, build, revision and guu info
            DisplayMessage += "\n<b>[GAME INFO]</b>\n";
            DisplayMessage += $"<b>Version:</b> {GuuCore.GAME_VERSION}  <b>Build:</b> {GuuCore.GAME_BUILD}  <b>Revision:</b> {GuuCore.GAME_REVISION}\n";
            DisplayMessage += $"<b>Store:</b> {GuuCore.GAME_STORE}  <b>Platform:</b> {Application.platform}  <b>Engine:</b> Unity {Application.unityVersion}\n";
            DisplayMessage += $"<b>Guu Version:</b> {GuuCore.GUU_VERSION}  <b>Build Type:</b> {GuuCore.GUU_BUILD}\n";
            
            //& Generate mod information
            DisplayMessage += "\n<b>[TROUBLEMAKER INFO]</b>\n";
            DisplayMessage += $"{TraceMod(exception)}\n";
            
            //& Add the message and trace from the exception
            DisplayMessage += "\n<b>[EXCEPTION]</b>\n";
            DisplayMessage += $"<color=yellow>{(!exception.Message.Matches(@"^(?:(?!Exception:).)*Exception:(?!.*Exception:).*$") ? exception.GetType().Name + ": " : string.Empty)}{InternalLogger.ReplaceInnerIfPresent(exception.Message, exception.InnerException)}</color>\n{HighlightTrace(StackTracing.ParseStackTrace(new StackTrace(exception, true)))}";
            
            // Process if it is a ReflectionTypeLoadException
            string indent = "  ";
            DisplayMessage += ProcessLoaderException(exception, indent);
            
            // Process inner exceptions
            indent = "  ";
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                DisplayMessage += $"\n{indent}<b>-- INNER EXCEPTION --</b>\n";
                DisplayMessage += $"{indent}<color=yellow>{(!exception.Message.Matches(@"^(?:(?!Exception:).)*Exception:(?!.*Exception:).*$") ? exception.GetType().Name + ": " : string.Empty)}{InternalLogger.ReplaceInnerIfPresent(exception.Message, exception.InnerException)}</color>\n{indent}{HighlightTrace(StackTracing.ParseStackTrace(new StackTrace(exception, true))).Replace("\n", $"\n{indent}")}";
                DisplayMessage += ProcessLoaderException(exception, indent);
                indent += indent;
            }
            
            //& Writes the report into it's file
            FileInfo report = new FileInfo(REPORT_FILE.Replace("{dateTime}", $"{DateTime.Now:dd-MM-yyyy HH-mm-ss}"));
            using (StreamWriter writer = report.CreateText())
            {
                writer.Write(DisplayMessage.StripRichText());
                writer.Flush();
            }
            
            // Deletes the oldest report if there are more than the max allowed
            DeleteOldFiles();
        }
        
        //+ HELPERS
        private static string HighlightTrace(string trace)
        {
            if (trace == null) return string.Empty;

            string final = string.Empty;
            foreach (string line in trace.Split('\n', '\r'))
            {
                Match match = Regex.Match(line, @"(?<=  at )([\w\.\+<>\[\],`]+)::(.+)(?:\(.*\)) \[(.+)\] in (.+)\[(.+)\]");
                if (!match.Success)
                {
                    final += line + "\n";
                    continue;
                }

                final += line.Replace(match.Groups[1].Value, $"<color=cyan>{match.Groups[1].Value}</color>")
                             .Replace(match.Groups[2].Value, $"<color=orange>{match.Groups[2].Value}</color>")
                             .Replace(match.Groups[3].Value, $"<color=red>{match.Groups[3].Value}</color>")
                             .Replace(match.Groups[4].Value, $"<color=lime>{match.Groups[4].Value}</color>")
                             .Replace(match.Groups[5].Value, $"<color=cyan>{match.Groups[5].Value}</color>") + "\n";
            }

            return final;
        }
            
        private static void DeleteOldFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(GuuCore.REPORT_FOLDER);
            foreach (FileInfo file in dir.GetFiles($"*{Path.GetExtension(GuuCore.REPORT_NAME)}", SearchOption.TopDirectoryOnly)
                                         .OrderByDescending(info => info.LastWriteTime).Skip(MAX_REPORTS))
            {
                file.Delete();
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static string TraceMod(Exception exception)
        {
            if (exception == null) return "The troublemaker information couldn't be found";
            
            MatchCollection types = Regex.Matches(exception.StackTrace ?? string.Empty, @"(\w*[\.\+<>])+\w*(?=\s?\()");
            Assembly assembly = Assembly.GetCallingAssembly();
            Type type = null;
            
            // Search for the type
            foreach (Match typeName in types)
            {
                string fixType = Regex.Replace(typeName.Value, @"\+(\w*[\.\+<>])+\w*$|\.+(\w+)$", string.Empty);
                type = TypeUtils.GetTypeBySearch(fixType);
                
                if (type == null) continue;
                if (type.Assembly != assembly) break;
            }
            
            // Keeps searching for Inner Exceptions if no valid thing is found
            if (type == null && exception.InnerException != null)
            {
                while (exception?.InnerException != null)
                {
                    exception = exception.InnerException;
                    if (exception?.StackTrace == null) break;
                    
                    types = Regex.Matches(exception.StackTrace, @"(\w*[\.\+<>])+\w*(?=\s?\()");
            
                    // Search for the type
                    foreach (Match typeName in types)
                    {
                        string fixType = Regex.Replace(typeName.Value, @"\+(\w*[\.\+<>])+\w*$|\.+(\w+)$", string.Empty);
                        type = TypeUtils.GetTypeBySearch(fixType);
                
                        if (type == null) continue;
                        if (type.Assembly != assembly) break;
                    }
                }
            }
            
            // No type found
            if (type == null) return "The troublemaker information couldn't be found";

            // A mod found
            GuuMod mod = ModLoader.MOD_CONTEXTS.ContainsKey(type.Assembly) ? ModLoader.MOD_CONTEXTS[type.Assembly].Mod : null;
            if (mod != null) return $"<b>Name:</b> {mod.Name} ({mod.ID})  <b>Version:</b> {mod.Version}  <b>Author:</b> {mod.Author}\n<b>Unsafe:</b> {mod.IsUnsafe.ToHumanString()}";

            // Guu assembly found
            bool isGuu = GuuCore.GUU_MODULES.Contains(type.Assembly.GetName().Name);
            if (isGuu) return $"<b>Assembly</b> {type.Assembly.GetName().Name} (Guu)  <b>Version:</b> {GuuCore.GUU_VERSION}  <b>Build Type:</b> {GuuCore.GUU_BUILD}\n<b>Unsafe:</b> Yes";

            // Unity assembly found
            bool isUnity = type.Assembly.GetName().Name.StartsWith("Unity.") || type.Assembly.GetName().Name.StartsWith("UnityEngine.");
            if (isUnity) return $"<b>Assembly</b> {type.Assembly.GetName().Name} (Unity)  <b>Version:</b> {GuuCore.UNITY_VERSION}";

            // Game assembly found
            bool isGame = type.Assembly.GetName().Name.StartsWith("Assembly-");
            if (isGame) return $"<b>Assembly</b> {type.Assembly.GetName().Name} (Game)  <b>Version:</b> {Application.version}  <b>Platform:</b> {Application.platform}";
                
            // SRML assembly found
            //bool isSRML = ModLoader.IsSRMLAssembly(type.Assembly);
            // TODO: Make SRML checks
                
            // Other assembly found
            return $"<b>Assembly:</b> {type.Assembly.GetName().Name} ({type.Assembly.GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company ?? VALUE_UNKOWN})  " +
                   $"<b>Version:</b> {type.Assembly.GetAssemblyAttribute<AssemblyVersionAttribute>()?.Version ?? VALUE_UNKOWN}  " +
                   $"<b>Title:</b> {type.Assembly.GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title ?? VALUE_UNKOWN}  " +
                   $"<b>Product:</b> {type.Assembly.GetAssemblyAttribute<AssemblyProductAttribute>()?.Product ?? VALUE_UNKOWN}";
        }

        private static string ProcessLoaderException(Exception exception, string indent)
        {
            if (!(exception is ReflectionTypeLoadException loadEx)) return string.Empty;
            
            string @return = string.Empty;
            foreach (Exception ex in loadEx.LoaderExceptions)
            {
                @return += $"\n{indent}<b>-- LOADER EXCEPTION --</b>\n";
                @return += $"{indent}<color=yellow>{(!ex.Message.Matches(@"^(?:(?!Exception:).)*Exception:(?!.*Exception:).*$") ? ex.GetType().Name + ": " : string.Empty)}{ex.Message}</color>\n{indent}{HighlightTrace(StackTracing.ParseStackTrace(new StackTrace(ex, true))).Replace("\n", $"\n{indent}")}";
            }

            return @return;
        }
    }
}