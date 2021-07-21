using UnityEngine;

namespace Guu.API
{
	/// <summary>The builder for an api item</summary>
	public interface IBuilder
	{
		/// <summary>The base item to use when building the prefab</summary>
		GameObject BaseItem { get; }
		
		/// <summary>
		/// Builds the api item during instantiation
		/// </summary>
		/// <param name="prefab">The base prefab to build/populate</param>
		void Build(GameObject prefab);
	}
}