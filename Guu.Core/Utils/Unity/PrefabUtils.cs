using UnityEngine;

namespace Guu.Utils
{
	/// <summary>
	/// An utility class to help with Prefabs
	/// </summary>
	public static class PrefabUtils
	{
		//+ MODDED PREFAB CLASS
		/// <summary>
		/// The component for a modded prefab
		/// </summary>
		public class ModdedPrefab : Component { }
		
		//+ PREFAB CONTROL
		/// <summary>
		/// Builds a prefab from scratch
		/// </summary>
		/// <param name="name">The name of the prefab</param>
		/// <returns>The resulting prefab</returns>
		public static GameObject BuildPrefab(string name)
		{
			GameObject prefab = new GameObject(name);
			Object.DontDestroyOnLoad(prefab);
			prefab.AddComponent<ModdedPrefab>();
			prefab.SetActive(false);

			return prefab;
		}
		
		/// <summary>
		/// Builds a prefab from another prefab
		/// </summary>
		/// <param name="name">The name of the prefab</param>
		/// <param name="original">The original prefab to copy</param>
		/// <returns>The resulting prefab</returns>
		public static GameObject BuildPrefab(string name, GameObject original)
		{
			bool originState = original.activeSelf;
			original.SetActive(false);
			
			GameObject prefab = Object.Instantiate(original);
			prefab.name = name;
			Object.DontDestroyOnLoad(prefab);
			prefab.AddComponent<ModdedPrefab>();
			
			original.SetActive(originState);
			return prefab;
		}
	}
}
