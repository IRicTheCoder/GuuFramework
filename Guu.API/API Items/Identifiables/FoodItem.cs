using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Guu.API
{
	/// <summary>Allows you to make a food item to be used by the API</summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class FoodItem : IdentifiableItem<FoodItem>
	{
		//+ BASE SETTINGS
		private const string MODEL_BASE_NAME = "model_base";
		private static readonly GameObject BASE_ITEM = SRObjects.Get<GameObject>("tofuSpicy");

		/// <inheritdoc />
		protected override string NamePrefix => "food";
		
		//+ VARIABLES
		/// <summary>The mesh to be used by this food</summary>
		public Mesh Mesh { get; set; } = null;

		/// <summary>Should the food be vacuumable when ripe?</summary>
		public bool VacWhenRipe { get; set; } = true;

		/// <summary>The number of game hours on Unripe state</summary>
		public float UnripeGameHours { get; set; } = 6f;
		
		/// <summary>The number of game hours on Ripe state</summary>
		public float RipeGameHours { get; set; } = 6f;
		
		/// <summary>The number of game hours on Edible state</summary>
		public float EdibleGameHours { get; set; } = 36f;
		
		/// <summary>The number of game hours on Rotten state</summary>
		public float RottenGameHours { get; set; } = 6f;

		/// <summary>The material for the rotten state, set to null to use the default one</summary>
		public Material RottenMat { get; set; } = null;

		/// <summary>The scale for the food's model</summary>
		public float ModelScale { get; set; } = 0.2f;

		/// <summary>The material for this food's model</summary>
		public Material ModelMat { get; set; } = null;
		
		/// <summary>The function to apply the collider, set to null for default one</summary>
		public Action<GameObject, FoodItem> ApplyCollider { get; set; } = null;
		
		/// <summary>The food groups to add this food to</summary>
		public SlimeEat.FoodGroup[] FoodGroups { get; set; } = new SlimeEat.FoodGroup[0];
		
		//+ PROPERTIES
		/// <inheritdoc />
		public override GameObject BaseItem => BASE_ITEM;

		//+ METHODS
		/// <inheritdoc />
		public override void Build(GameObject prefab)
		{
			//? In case of questions, tofu keeps the model object as 'model_pogofruit'
			GameObject model = prefab.FindChild("model_pogofruit");
			model.name = MODEL_BASE_NAME;
			model.transform.localScale = Vector3.one * ModelScale;
			
			// Manages Collisions
			if (ApplyCollider != null)
				ApplyCollider.Invoke(prefab, this);
			else
			{
				Object.Destroy(prefab.GetComponent<BoxCollider>());
				SphereCollider col = prefab.AddComponent<SphereCollider>();
				col.radius = ModelScale;
			}
			
			// Loading components
			MeshFilter filter = prefab.GetComponent<MeshFilter>();
			Rigidbody body = prefab.GetComponent<Rigidbody>();
			Vacuumable vac = prefab.GetComponent<Vacuumable>();
			Identifiable iden = prefab.GetComponent<Identifiable>();

			ResourceCycle cycle = prefab.GetComponent<ResourceCycle>();

			MeshFilter mFilter = model.GetComponent<MeshFilter>();
			MeshRenderer render = model.GetComponent<MeshRenderer>();
			
			// Setting up components
			filter.sharedMesh = Mesh;
			body.mass = Mass;
			vac.size = VacSize;
			iden.id = ID;

			cycle.unripeGameHours = UnripeGameHours;
			cycle.ripeGameHours = RipeGameHours;
			cycle.edibleGameHours = EdibleGameHours;
			cycle.rottenGameHours = RottenGameHours;
			cycle.rottenMat = RottenMat ?? cycle.rottenMat;
			cycle.vacuumableWhenRipe = VacWhenRipe;

			mFilter.sharedMesh = Mesh;
			render.sharedMaterial = ModelMat;
		}

		/// <inheritdoc />
		public override void Register()
		{
			base.Register();

			// TODO: Add registration for food groups
		}
	}
}