using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Guu.Assets
{
	/// <summary>
	/// An asset pack that contains all assets in a bundle
	/// </summary>
	public class AssetPack
	{
		/// <summary>The asset bundle for this asset pack</summary>
		public AssetBundle Bundle { get; }

		/// <summary>The list of all objects in the bundle</summary>
		public List<Object> BundleObjects { get; } = new List<Object>();

		/// <summary>
		/// Creates a new asset pack with a bundle in the given path
		/// </summary>
		/// <param name="path">Path to the bundle</param>
		public AssetPack(string path) : this(AssetBundle.LoadFromFile(path)) { }

		/// <summary>
		/// Creates a new asset pack with the given bundle
		/// </summary>
		/// <param name="bundle">The bundle to use</param>
		private AssetPack(AssetBundle bundle)
		{
			Bundle = bundle;
			BundleObjects.AddRange(Bundle.LoadAllAssets());
		}

		/// <summary>
		/// Gets an object from the asset pack
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="name">Name of the object</param>
		/// <returns>The object or null if nothing is found</returns>
		public T Get<T>(string name) where T : Object
		{
			foreach (T obj in BundleObjects.Where(o => o.GetType() == typeof(T)).Cast<T>())
			{
				if (obj.name.Equals(name))
					return obj;
			}

			return null;
		}

		/// <summary>
		/// Gets all assets of a type from the asset pack
		/// </summary>
		/// <typeparam name="T">Type of objects</typeparam>
		/// <returns>An array with all the objects found</returns>
		public T[] GetAll<T>() where T : Object
		{
			return BundleObjects.Where(o => o.GetType() == typeof(T)).Cast<T>().ToArray();
		}
	}
}
