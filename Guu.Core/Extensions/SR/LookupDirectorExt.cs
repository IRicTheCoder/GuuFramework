using System.Collections.Generic;
using UnityEngine;

// TODO: Make the entire extension in this class
// TODO: Revamp old one

namespace Guu
{
	/// <summary>
	/// Contains extension methods for the LookupDirector
	/// </summary>
	public static class LookupDirectorExt
	{
		//+ PREFAB CONTROL
		/// <summary>
		/// Gets all the prefabs from the given IDs
		/// </summary>
		/// <param name="dir">The lookup director</param>
		/// <param name="ids">The IDs</param>
		public static GameObject[] GetPrefabs(this LookupDirector dir, Identifiable.Id[] ids)
		{
			List<GameObject> objs = new List<GameObject>();
			foreach (Identifiable.Id id in ids)
				objs.Add(dir.GetPrefab(id));

			return objs.ToArray();
		}

		/// <summary>
		/// Checks if there is a prefab for the given identifiable ID
		/// </summary>
		/// <param name="dir">The lookup director</param>
		/// <param name="id">The ID to check</param>
		public static bool HasPrefab(this LookupDirector dir, Identifiable.Id id)
		{
			return dir.GetPrivateField<Dictionary<Identifiable.Id, GameObject>>("identifiablePrefabDict").ContainsKey(id);
		}
		
		/// <summary>
		/// Checks if there is a prefab for the given land plot ID
		/// </summary>
		/// <param name="dir">The lookup director</param>
		/// <param name="id">The ID to check</param>
		public static bool HasPlotPrefab(this LookupDirector dir, LandPlot.Id id)
		{
			return dir.GetPrivateField<Dictionary<LandPlot.Id, GameObject>>("plotPrefabDict").ContainsKey(id);
		}
	}
}