using System;

namespace Guu
{
    /// <summary>
    /// Identifies a module for your mod, added to the ModMain class from your mod to identify one module.
    /// Add multiple for multiple modules.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ModModuleAttribute : Attribute
    {
        //+ VARIABLES
        internal string moduleName;
        internal string dependentID;
        internal string dependentVersion;

        //+ CONSTRUCTOR
        /// <summary>
        /// Defines this module
        /// </summary>
        /// <param name="moduleName">The name of the module (the same name the .dll file has)</param>
        /// <param name="dependentID">The ID of the mod this is dependent on, either null or your mod id to use as a module only dependent of your mod</param>
        /// <param name="dependentVersion">The version the dependent needs to have, use a "*" in the place of a number to account for any number.
        /// Only the major version number is required</param>
        public ModModuleAttribute(string moduleName, string dependentID = null, string dependentVersion = "*")
        {
            this.moduleName = moduleName;
            this.dependentID = dependentID;
            this.dependentVersion = dependentVersion;
        }
    }
}