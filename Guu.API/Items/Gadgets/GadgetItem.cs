using UnityEngine;

namespace Guu.API.Gadgets
{
	/// <summary>
	/// This is the base class for all gadget items
	/// </summary>
	public abstract class GadgetItem : RegistryItem<GadgetItem>
	{
		//+ Instance Bound
		protected GameObject Prefab { get; set; }
		private GadgetDefinition Definition { get; set; }
		
		//+ Abstracts
		protected abstract Gadget.Id ID { get; }
		protected abstract PediaDirector.Id PediaID { get; }
		
		//+ Virtual
		protected virtual bool DestroyOnRemoval { get; } = false;
		protected virtual bool BuyInPairs { get; } = false;
		protected virtual bool StartAvailable { get; } = false;
		protected virtual bool StartUnlocked { get; } = false;
		
		protected virtual int BlueprintCost { get; } = 0;
		protected virtual int BuyLimit { get; } = 0;
		protected virtual int CountLimit { get; } = 0;
		
		protected virtual Sprite Icon { get; } = null;
		
		protected virtual GadgetType[] Types { get; } = { GadgetType.MISC };
		
		protected virtual Gadget.Id[] CountIDs { get; } = null;
		protected virtual GadgetDefinition.CraftCost[] CraftCosts { get; } = null;

		//+ Methods
		protected virtual GadgetDirector.BlueprintLocker CreateBlueprintLocker(GadgetDirector dir) => null;

		/// <summary>Registers the item into it's registry</summary>
		public override GadgetItem Register()
		{
			Definition = ScriptableObject.CreateInstance<GadgetDefinition>();
			Definition.id = ID;
			Definition.prefab = Prefab;
			Definition.icon = Icon;
			Definition.pediaLink = PediaID;
			Definition.blueprintCost = BlueprintCost;
			Definition.craftCosts = CraftCosts;
			Definition.buyCountLimit = BuyLimit;
			Definition.countLimit = CountLimit;
			Definition.countOtherIds = CountIDs;
			Definition.destroyOnRemoval = DestroyOnRemoval;
			Definition.buyInPairs = BuyInPairs;
			Definition.name = NamePrefix + Name;

			Build();

			GadgetRegistry.Classify(Definition, Types);
			GadgetRegistry.RegisterGadget(Definition, StartAvailable, StartUnlocked);
			
			if (!StartAvailable)
				GadgetRegistry.RegisterLock(Definition, CreateBlueprintLocker);

			return this;
		}

		protected GadgetDefinition.CraftCost CraftCost(Identifiable.Id id, int amount)
		{
			return new GadgetDefinition.CraftCost()
			{
				id = id,
				amount = amount >= 999 ? 999 : amount
			};
		}
	}
}
