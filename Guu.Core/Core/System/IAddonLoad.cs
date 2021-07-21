namespace Guu
{
    /// <summary>
    /// Represents the entry point or loading point of an addon assembly
    /// </summary>
    public interface IAddonLoad
    {
        /// <summary>Initializes the Addon</summary>
        void Initialize();
    }
}