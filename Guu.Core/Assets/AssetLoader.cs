using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Guu.Utils;

namespace Guu.Assets
{
	/// <summary>
	/// Loads assets from asset packs
	/// </summary>
	public static class AssetLoader
	{
		/// <summary>
		/// Loads an asset pack from path
		/// </summary>
		/// <param name="path">Path to the bundle</param>
		public static AssetPack LoadPack(string path)
		{
			return new AssetPack(path);
		}

		/// <summary>
		/// Loads an asset pack from the mod's folder
		/// </summary>
		/// <param name="relPath">Relative path to the pack</param>
		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		public static AssetPack LoadPackRelative(string relPath)
		{
			Assembly assembly = Loader.ModLoader.Context.Main.Assembly;
			string codeBase = assembly.CodeBase;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path)), "Resources/Assets");

			return new AssetPack(Path.Combine(path, relPath));
		}
	}
}
