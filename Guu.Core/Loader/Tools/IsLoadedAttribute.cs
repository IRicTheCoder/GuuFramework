using System;

namespace Guu
{
    /// <summary>
    /// Marks a field to be changed with true or false based if a mod is loaded or not
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IsLoadedAttribute : Attribute
    {
        internal string modID;

        /// <summary>
        /// Marks a field to be changed with true or false based if a mod is loaded or not
        /// </summary>
        /// <param name="modID">The mod ID to check (use 'srml:' before the ID to check for SRML mods, use 'assem:' to
        /// check for any other kind of mods). When using 'assem:' the ID is the name of the assembly.</param>
        public IsLoadedAttribute(string modID)
        {
            this.modID = modID;
        }
    }
}