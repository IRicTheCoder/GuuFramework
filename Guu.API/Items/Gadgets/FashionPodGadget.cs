using Guu.Utils;
using UnityEngine;

namespace Guu.API.Gadgets
{
	/// <summary>
	/// This is the base class to make decoration gadgets
	/// </summary>
	public abstract class FashionPodGadget : GadgetItem
	{
		// Base item to create this one
		private static GameObject BaseItem => GameContext.Instance.LookupDirector?.GetGadgetDefinition(Gadget.Id.FASHION_POD_CLIP_ON).prefab ?? 
		                                      SRObjects.Get<GameObject>("gadgetFashionPodClipOn");
		
		// Overrides
		protected override string NamePrefix => "gadgetFashionPod";
		protected override GadgetType[] Types { get; } = { GadgetType.FASHION_POD };

		// Abstracts
		protected abstract Identifiable.Id FashionID { get; }

		// Methods
		protected override void Build()
		{
			// Get GameObjects
			Prefab = PrefabUtils.BuildPrefab(NamePrefix + Name, BaseItem);

			// Load Components
			Gadget gadget = Prefab.GetComponent<Gadget>();
			FashionPod pod = Prefab.GetComponent<FashionPod>();

			// Setup Components
			gadget.id = ID;
			pod.fashionId = FashionID;
		}

		/// <summary>Registers the item into it's registry</summary>
		public override GadgetItem Register()
		{
			base.Register();

			return this;
		}
	}
}
