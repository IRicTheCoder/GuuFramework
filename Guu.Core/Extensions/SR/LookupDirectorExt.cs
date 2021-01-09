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
		//+ VARIABLES
		#pragma warning disable 649 /// Disables unassigned field
		private static Dictionary<Identifiable.Id, GameObject> identPrefabs;
		private static Dictionary<LandPlot.Id, GameObject> plotPrefabs;
		private static Dictionary<SpawnResource.Id, GameObject> spawnResPrefabs;
		private static Dictionary<Gadget.Id, GadgetDefinition> gadgetDefs;
		private static Dictionary<Identifiable.Id, VacItemDefinition> vacItemDefs;
		private static Dictionary<Identifiable.Id, LiquidDefinition> liquidDefs;
		private static Dictionary<PlayerState.Upgrade, UpgradeDefinition> upgradeDefs;
		private static Dictionary<Identifiable.Id, GameObject> gordoPrefabs;
		private static Dictionary<Identifiable.Id, ToyDefinition> toyDefs;
		#pragma warning restore 649

		//+ LOOKUP LISTINGS
		internal static void LoadListings(this LookupDirector dir)
		{
			identPrefabs.SetFromPrivate(dir, "identifiablePrefabDict");
			plotPrefabs.SetFromPrivate(dir, "plotPrefabDict");
			spawnResPrefabs.SetFromPrivate(dir, "resourcePrefabDict");
			gadgetDefs.SetFromPrivate(dir, "gadgetDefinitionDict");
			vacItemDefs.SetFromPrivate(dir, "vacItemDict");
			liquidDefs.SetFromPrivate(dir, "liquidDict");
			upgradeDefs.SetFromPrivate(dir, "upgradeDefinitionDict");
			gordoPrefabs.SetFromPrivate(dir, "gordoDict");
			toyDefs.SetFromPrivate(dir, "toyDict");
		}
		
		internal static Dictionary<Identifiable.Id, GameObject> IdentPrefabs(this LookupDirector dir) => identPrefabs;
		internal static Dictionary<LandPlot.Id, GameObject> PlotPrefabs(this LookupDirector dir) => plotPrefabs;
		internal static Dictionary<SpawnResource.Id, GameObject> SpawnResPrefabs(this LookupDirector dir) => spawnResPrefabs;
		internal static Dictionary<Gadget.Id, GadgetDefinition> GadgetDefs(this LookupDirector dir) => gadgetDefs;
		internal static Dictionary<Identifiable.Id, VacItemDefinition> VacItemDefs(this LookupDirector dir) => vacItemDefs;
		internal static Dictionary<Identifiable.Id, LiquidDefinition> LiquidDefs(this LookupDirector dir) => liquidDefs;
		internal static Dictionary<PlayerState.Upgrade, UpgradeDefinition> UpgradeDefs(this LookupDirector dir) => upgradeDefs;
		internal static Dictionary<Identifiable.Id, GameObject> GordoPrefabs(this LookupDirector dir) => gordoPrefabs;
		internal static Dictionary<Identifiable.Id, ToyDefinition> ToyDefs(this LookupDirector dir) => toyDefs;

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
			return identPrefabs.ContainsKey(id);
		}
		
		/// <summary>
		/// Checks if there is a prefab for the given land plot ID
		/// </summary>
		/// <param name="dir">The lookup director</param>
		/// <param name="id">The ID to check</param>
		public static bool HasPlotPrefab(this LookupDirector dir, LandPlot.Id id)
		{
			return plotPrefabs.ContainsKey(id);
		}
	}
}