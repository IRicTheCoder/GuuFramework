using System.Collections.Generic;
using System.IO;
using Guu.Utils;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace Guu.Loader
{
    /// <summary>
    /// Represents a mod loaded by Guu, contains some information referent to said mod.
    /// </summary>
    [UsedImplicitly]
    public class GuuMod
    {
        //+ CONSTANTS
        private const string UNKNOWN_AUTHOR = "Unknown";
        private const string UNKNOWN_DESCRIPTION = "No description was found!";
        
        //+ PROPERTIES
        /// <summary>The ID of this mod</summary>
        [YamlMember(Alias = "modID", ApplyNamingConventions = false)]
        public string ID { get; internal set; } = null;

        /// <summary>The ID of this mod</summary>
        public string Name { get; internal set; } = null;

        /// <summary>The version of this mod</summary>
        public string Version { get; internal set; } = null;

        /// <summary>The name of the assembly for this mod</summary>
        [YamlMember(Alias = "assembly", ApplyNamingConventions = false)] 
        public string AssemblyName { get; internal set; } = null;
        
        /// <summary>The guu version of this mod</summary>
        [YamlMember(Alias = "guuVersion", ApplyNamingConventions = false)] 
        public string GuuVersion { get; internal set; } = null;
        
        /// <summary>The author of this mod</summary>
        public string Author { get; internal set; } = UNKNOWN_AUTHOR;
        
        /// <summary>The description of this mod</summary>
        public string Description { get; internal set; } = UNKNOWN_DESCRIPTION;
        
        /// <summary>Does this mod use features marked as unsafe?</summary>
        [YamlMember(Alias = "isUnsafe", ApplyNamingConventions = false)]
        public bool IsUnsafe { get; internal set; } = false;

        /// <summary>The mods required for this mod to work</summary>
        [YamlMember(Alias = "requiredMods", ApplyNamingConventions = false)]
        public string[] RequiredMods { get; internal set; } = new string[0];
        
        /// <summary>The mods that need to load before this one</summary>
        [YamlMember(Alias = "loadBefore", ApplyNamingConventions = false)]
        public string[] LoadBefore { get; internal set; } = new string[0];
        
        /// <summary>The mods that need to load after this one</summary>
        [YamlMember(Alias = "loadAfter", ApplyNamingConventions = false)]
        public string[] LoadAfter { get; internal set; } = new string[0];
        
        //? Ignored
        /// <summary>The file that contains the info for this mod</summary>
        [YamlIgnore]
        public FileInfo Info { get; internal set; }
        
        /// <summary>The Modules loaded by this mod (Index 0 is the main module)</summary>
        [YamlIgnore]
        public IModLoad[] Modules { get; internal set; }
    }
}