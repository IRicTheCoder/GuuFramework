using Guu.Utils;
using UnityEngine;

namespace Guu.API.Gadgets
{
	/// <summary>
	/// This is the base class to make decoration gadgets
	/// </summary>
	public abstract class DecorGadget : GadgetItem
	{
		// Overrides
		protected override string NamePrefix => "decor";
		protected override GadgetType[] Types { get; } = { GadgetType.DECO };

		// Abstracts
		protected abstract GameObject BasePrefab { get; }

		// Methods
		protected override void Build()
		{
			// Get GameObjects
			Prefab = PrefabUtils.BuildPrefab(NamePrefix + Name, BasePrefab);

			// Load Components
			Gadget gadget = Prefab.AddComponent<Gadget>();

			// Setup Components
			gadget.id = ID;
		}
	}
}
