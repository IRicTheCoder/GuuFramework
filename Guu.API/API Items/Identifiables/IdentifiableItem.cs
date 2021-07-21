using Guu.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
	/// <summary>The base for identifiable items made by the API</summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public abstract class IdentifiableItem<T> : APIBase<T> where T : IdentifiableItem<T>
	{
		//+ PROPERTIES
		/// <summary>The ID of this Identifiable</summary>
		public Identifiable.Id ID { get; set; } = Identifiable.Id.NONE;

		/// <summary>The identifiable types this identifiable belongs to</summary>
		public IdentifiableType[] Types { get; set; } = new IdentifiableType[0];

		/// <summary>The entry for the VacPack</summary>
		public VacItemDefinition VacEntry { get; set; }

		/// <summary>The vac size of this Identifiable</summary>
		public Vacuumable.Size VacSize { get; set; } = Vacuumable.Size.NORMAL;

		/// <summary>Should this identifiable be registered as something the player can vac?</summary>
		public bool IsVacuumable { get; set; }

		/// <summary>Can this Identifiable be added to the refinery as a resource?</summary>
		public bool IsRefineryResource { get; set; }

		/// <summary>The main scale of the entire prefab</summary>
		public float Scale { get; set; } = 1f;

		/// <summary>The mass of this Identifiable</summary>
		public float Mass { get; set; } = 0.3f;

		/// <summary>The storage types where this identifiable can be stored</summary>
		public SiloStorage.StorageType[] StorageTypes { get; set; } = new SiloStorage.StorageType[0];
		
		// Extras

		//+ METHODS
		/// <inheritdoc />
		public override void Register()
		{
			//? Identifiable Registration
			IdentifiableRegistry.Classify(ID, Types);
			IdentifiableRegistry.RegisterPrefab(FakePrefab);
			IdentifiableRegistry.RegisterVacEntry(VacEntry);
			
			//? Ammo Registration
			if (IsVacuumable)
				AmmoRegistry.RegisterPlayerAmmo(PlayerState.AmmoMode.DEFAULT, ID);
			
			foreach (SiloStorage.StorageType type in StorageTypes)
				AmmoRegistry.RegisterStorageAmmo(type, ID);
			
			//? Refinery Registration
			if (IsRefineryResource)
				AmmoRegistry.RegisterRefineryResource(ID);
		}

		/// <inheritdoc />
		public override void Build(GameObject prefab)
		{
			prefab.transform.localScale = Vector3.one * Scale;
		}

		/// <inheritdoc />
		public override T Create()
		{
			base.Create();

			Identifiable iden = FakePrefab.AddComponent<Identifiable>();
			iden.id = ID;

			return (T) this;
		}
	}
}