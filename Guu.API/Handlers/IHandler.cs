namespace Guu.API
{
	/// <summary>Represents a handler for the registry system</summary>
	public interface IHandler<out T> where T : IHandler<T>
	{
		/// <summary>
		/// Sets up the handler
		/// </summary>
		/// <returns>Returns this handler for convenience</returns>
		T Setup();
		
		/// <summary>
		/// Handles the relevant items right after registration. This runs after everything is registered, to allow you to
		/// replace the relevant items if needed.
		/// </summary>
		void RegistryHandle();
		
		/// <summary>
		/// Handles the relevant items. This runs after everything is loaded, to allow you to
		/// manipulate the relevant items if needed, to adjust values or make inter mod stuff
		/// </summary>
		void Handle();

		/// <summary>
		/// Clear any values from the handler that are no longer used to save up memory
		/// </summary>
		void ClearMemory();
	}
}