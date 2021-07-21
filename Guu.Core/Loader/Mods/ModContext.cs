using System;
using Eden.Core.Context;
using Guu.Logs;

namespace Guu.Loader
{
	/// <summary>
	/// Represents the context of a mod
	/// </summary>
	public class ModContext : IContext
	{
		//+ PROPERTIES
		/// <summary>The mod being hold for context</summary>
		public GuuMod Mod { get; }
		
		/// <summary>The mod main of the mod being hold for context</summary>
		public ModMain Main { get; }

		/// <summary>The logger of the mod beig hold for context</summary>
		public ModLogger Logger => Main.Logger;
		
		//+ CONSTRUCTOR
		internal ModContext(GuuMod mod, ModMain main)
		{
			Mod = mod;
			Main = main;
		}

		//+ CONVERSION
		/// <inheritdoc/>
		public object As(Type type) => Convert.ChangeType(this, type);

		/// <inheritdoc/>
		public T As<T>() where T : class, IContext => this as T;
	}
}