using System.Reflection;

namespace Guu
{
    /// <summary>
    /// Represents a class used during mod loading
    /// </summary>
    public interface IModLoad
    {
        /// <summary>The main assembly for this mod</summary>
        Assembly Assembly { get; }

        /// <summary>Runs the current load step</summary>
        void RunLoadStep();
    }
}