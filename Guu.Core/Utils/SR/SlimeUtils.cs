﻿using System.Collections.Generic;
using System.Linq;
using Eden.Core.Utils;
using Eden.Patching.Fixers;
using UnityEngine;

using static SlimeEat;

namespace Guu.Utils
{
	/// <summary>
	/// An utility class to help with Slimes
	/// </summary>
	public static class SlimeUtils
	{
		/// <summary>A constant that represents the min. drive to eat</summary>
		public const float EAT_MIN_DRIVE = 0f;

		/// <summary>A constant that represents the min. drive to eat plorts</summary>
		public const float EAT_PLORT_MIN_DRIVE = 0.5f;

		/// <summary>A constant that represents the default extra drive in diets</summary>
		public const float DEFAULT_EXTRA_DRIVE = 0f;

		/// <summary>A constant that represents the default favorite production count</summary>
		public const int DEFAULT_FAV_COUNT = 2;

		/// <summary>
		/// Add food to a food group for all slimes inside that group
		/// </summary>
		/// <param name="id">ID of the food to add</param>
		/// <param name="group">Group of food to add in</param>
		/// <param name="favoritedBy">The IDs for the slimes that favorite this food</param>
		public static void AddFoodToGroup(Identifiable.Id id, FoodGroup group, ICollection<Identifiable.Id> favoritedBy = null)
		{
			foreach (SlimeDefinition def in GameContext.Instance.SlimeDefinitions.Slimes)
			{
				if (def.Diet == null || !HasFoodGroup(def.Diet, group) || def.Diet.EatMap == null || def.Diet.EatMap.Count <= 0)
					continue;

				if (def.Diet.EatMap.Exists(e => e.eats == id))
					continue;

				def.Diet.EatMap.Add(new SlimeDiet.EatMapEntry()
				{
					becomesId = Identifiable.Id.NONE,
					driver = SlimeEmotions.Emotion.HUNGER,
					eats = id,
					extraDrive = DEFAULT_EXTRA_DRIVE,
					favoriteProductionCount = DEFAULT_FAV_COUNT,
					isFavorite = favoritedBy?.Contains(def.IdentifiableId) ?? false,
					minDrive = EAT_MIN_DRIVE,
					producesId = SlimeToPlort(def.IdentifiableId)
				});
			}
		}

		/// <summary>
		/// Add food to all food groups for all slimes
		/// </summary>
		/// <param name="id">ID of the food to add</param>
		/// <param name="favoritedBy">The IDs for the slimes that favorite this food</param>
		/// <param name="blackList">Black list of slimes to ignore</param>
		public static void AddFoodToAllGroups(Identifiable.Id id, ICollection<Identifiable.Id> favoritedBy = null, ICollection<Identifiable.Id> blackList = null)
		{
			foreach (SlimeDefinition def in GameContext.Instance.SlimeDefinitions.Slimes)
			{
				if (def.Diet == null || (blackList?.Contains(def.IdentifiableId) ?? false) || def.Diet.EatMap == null || def.Diet.EatMap.Count <= 0)
					continue;

				if (def.Diet.EatMap.Exists((e) => e.eats == id))
					continue;

				def.Diet.EatMap.Add(new SlimeDiet.EatMapEntry()
				{
					becomesId = Identifiable.Id.NONE,
					driver = SlimeEmotions.Emotion.HUNGER,
					eats = id,
					extraDrive = DEFAULT_EXTRA_DRIVE,
					favoriteProductionCount = DEFAULT_FAV_COUNT,
					isFavorite = favoritedBy?.Contains(def.IdentifiableId) ?? false,
					minDrive = EAT_MIN_DRIVE,
					producesId = SlimeToPlort(def.IdentifiableId)
				});
			}
		}

		/// <summary>
		/// Does this diet belongs to a given food group?
		/// </summary>
		/// <param name="diet">Diet to check</param>
		/// <param name="group">Group to check</param>
		private static bool HasFoodGroup(SlimeDiet diet, FoodGroup group)
		{
			return diet.MajorFoodGroups.Any(food => food == group);
		}

		/// <summary>
		/// Gets all slime diets from a Food Group
		/// </summary>
		/// <param name="group">Group to search for</param>
		public static List<SlimeDiet> GetFoodGroupDiets(FoodGroup group)
		{
			List<SlimeDiet> diets = new List<SlimeDiet>();

			foreach (SlimeDefinition def in GameContext.Instance.SlimeDefinitions.Slimes)
			{
				if (def.Diet == null)
					continue;

				if (HasFoodGroup(def.Diet, group))
					diets.Add(def.Diet);
			}

			return diets;
		}

		/// <summary>
		/// Populates a diet with the given foods
		/// </summary>
		/// <param name="slimeID">Slime ID that owns the diet</param>
		/// <param name="foods">Foods to add</param>
		/// <param name="diet">The diet to populate</param>
		/// <param name="favoriteFoods">A list of favorite foods (null for no favorite foods)</param>
		/// <param name="plort">The plort to produce</param>
		/// <param name="plortAsFood">Are plorts food for this slime?</param>
		public static void PopulateDiet(Identifiable.Id slimeID, ICollection<Identifiable.Id> foods, SlimeDiet diet, ICollection<Identifiable.Id> favoriteFoods = null, Identifiable.Id? plort = null, bool plortAsFood = false)
		{
			if (diet.EatMap == null)
				diet.EatMap = new List<SlimeDiet.EatMapEntry>();

			Identifiable.Id slimePlort = plort ?? SlimeToPlort(slimeID);

			foreach (Identifiable.Id food in foods)
			{
				if (diet.EatMap.Exists((e) => e.eats == food))
					continue;

				if (Identifiable.IsPlort(food) && !plortAsFood && PlortToLargo(slimePlort, food) == Identifiable.Id.NONE)
					continue;

				diet.EatMap.Add(new SlimeDiet.EatMapEntry()
				{
					becomesId = (Identifiable.IsPlort(food) && !plortAsFood) 
						? PlortToLargo(slimePlort, food)
						: Identifiable.Id.NONE,

					driver = SlimeEmotions.Emotion.HUNGER,
					eats = food,
					extraDrive = DEFAULT_EXTRA_DRIVE,
					favoriteProductionCount = DEFAULT_FAV_COUNT,
					isFavorite = favoriteFoods?.Contains(food) ?? false,
					minDrive = (Identifiable.IsPlort(food) && !plortAsFood) ? EAT_PLORT_MIN_DRIVE : EAT_MIN_DRIVE,
					producesId = (Identifiable.IsPlort(food) && !plortAsFood) ? Identifiable.Id.NONE 
						: slimePlort
				});
			}
		}

		/// <summary>
		/// Gets the Plort for a Slime
		/// </summary>
		/// <param name="slime">ID of the slime</param>
		public static Identifiable.Id SlimeToPlort(Identifiable.Id slime)
		{
			SlimeDefinition def = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(slime);

			return def.Diet?.EatMap == null ? 
				Identifiable.Id.NONE : 
				(from entry in def.Diet.EatMap where Identifiable.IsPlort(entry.producesId) select entry.producesId).FirstOrDefault();
		}

		/// <summary>
		/// Gets a Largo from plorts
		/// </summary>
		/// <param name="plortA">ID of the first plort</param>
		/// <param name="plortB">ID of the second plort</param>
		public static Identifiable.Id PlortToLargo(Identifiable.Id plortA, Identifiable.Id plortB)
		{
			return GameContext.Instance.SlimeDefinitions.GetLargoByPlorts(plortA, plortB)?.IdentifiableId ?? Identifiable.Id.NONE;
		}

		/// <summary>
		/// Gets a Slime Definition by ID (force if needed)
		/// </summary>
		/// <param name="id">ID of the slime</param>
		public static SlimeDefinition GetDefinitionByID(Identifiable.Id id)
		{
			if (GameContext.Instance != null)
				return GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(id);

			return SRObjects.GetAll<SlimeDefinition>().FirstOrDefault(def => def.IdentifiableId == id);
		}
		
		/// <summary>
		/// Generates all the IDs for the custom largos. (THIS CAN ONLY RUN ON PRE-LOAD)
		/// <para>NOTE that all IDs are generated even if they don't get used.</para>
		/// </summary>
		/// <param name="slimeToLargo">ID of the slime to create custom largos</param>
		public static void GenerateLargoIDs(Identifiable.Id slimeToLargo)
		{
			foreach (Identifiable.Id id in Identifiable.SLIME_CLASS)
			{
				if (GetLargoID(slimeToLargo, id) != Identifiable.Id.NONE && 
				    GetLargoID(slimeToLargo, id, "LARGO") != Identifiable.Id.NONE)
					continue;

				string name = $"{slimeToLargo.ToString().Replace("_SLIME", "")}_{id.ToString().Replace("_SLIME", "")}_CLARGO";

				// TODO: Fix this
				//Identifiable.Id newVal = EnumFixer.AddValue<Identifiable.Id>(name);
				//IdentifiableRegistry.CreateIdentifiableId(newVal, name, false);
			}
		}

		// TODO: Make the get largo ID use both suffixes
		/// <summary>
		/// Gets a Custom Largo ID (With suffix _CLARGO)
		/// </summary>
		/// <param name="slimeA">ID of base slime A</param>
		/// <param name="slimeB">ID of base slime B</param>
		/// <param name="suffix">The suffix of the largo ID</param>
		public static Identifiable.Id GetLargoID(Identifiable.Id slimeA, Identifiable.Id slimeB, string suffix = "CLARGO")
		{
			string name = $"{slimeA.ToString().Replace("_SLIME", "")}_{slimeB.ToString().Replace("_SLIME", "")}_{suffix}";
			string name2 = $"{slimeB.ToString().Replace("_SLIME", "")}_{slimeA.ToString().Replace("_SLIME", "")}_{suffix}";

			Identifiable.Id newID = Largo(name);

			if (newID != Identifiable.Id.NONE)
				return newID;

			newID = Largo(name2);

			return newID != Identifiable.Id.NONE ? newID : Identifiable.Id.NONE;
		}

		/// <summary>
		/// Gets a Largo from the given slimes
		/// </summary>
		/// <param name="slimeA">ID of slime A</param>
		/// <param name="slimeB">ID of slime B</param>
		public static Identifiable.Id Largo(Identifiable.Id slimeA, Identifiable.Id slimeB)
		{
			SlimeDefinitions defs = GameContext.Instance.SlimeDefinitions;
			return defs.GetLargoByBaseSlimes(defs.GetSlimeByIdentifiableId(slimeA), defs.GetSlimeByIdentifiableId(slimeB))?.IdentifiableId ?? Identifiable.Id.NONE;
		}

		/// <summary>
		/// Gets a Largo from a given ID name
		/// </summary>
		/// <param name="name">ID name to search for</param>
		private static Identifiable.Id Largo(string name)
		{
			return EnumUtils.Parse<Identifiable.Id>(name);
		}

		// TODO: Check this method later
		/*public static Sprite GetLargoIcon(Sprite slimeA, Sprite slimeB)
		{
			Texture2D result = new Texture2D(512, 512);

			Texture2D texA = slimeA.texture;
			Texture2D texB = new Texture2D(512, 512);
			texB.SetPixels(0, 0, 512, 512, slimeB.texture.GetPixels(0, 0, 512, 512));
			texB.Apply();
			texB.Resize(256, 256, TextureFormat.DXT5Crunched, true);
			texB.Apply();

			result.SetPixels(0, 0, 512, 512, texA.GetPixels(0, 0, 512, 512));
			result.SetPixels(255, 255, 512, 512, texB.GetPixels(0, 0, 256, 256));
			result.Apply();

			return Sprite.Create(result, new Rect(Vector2.zero, Vector2.one * 512), Vector2.one * 256, 50, 1, SpriteMeshType.Tight);
		}*/

		/// <summary>
		/// Clones an Appearance Structure the right way
		/// </summary>
		/// <param name="old">The old appearance to clone</param>
		/// <returns>The new cloned appearance</returns>
		public static SlimeAppearanceStructure CloneAppearanceStructure(SlimeAppearanceStructure old)
		{
			// The cloned app
			SlimeAppearanceStructure newApp = new SlimeAppearanceStructure(old)
			{
				DefaultMaterials = new Material[old.DefaultMaterials.Length],
				ElementMaterials = new SlimeAppearanceMaterials[old.ElementMaterials.Length],
				Element = ScriptableObject.CreateInstance<SlimeAppearanceElement>(),
				SupportsFaces = old.SupportsFaces,
				FaceRules = new SlimeFaceRules[old.FaceRules.Length]
			};

			// Adjust other values
			newApp.Element.Name = old.Element.Name;
			newApp.Element.Prefabs = old.Element.Prefabs;
			
			for (int i = 0; i < old.FaceRules.Length; i++)
			{
				newApp.FaceRules[i].ShowEyes = old.FaceRules[i].ShowEyes;
				newApp.FaceRules[i].ShowMouth = old.FaceRules[i].ShowMouth;
			}
			
			// Clone the Materials
			for (int i = 0; i < old.DefaultMaterials.Length; i++)
				newApp.DefaultMaterials[i] = Object.Instantiate(old.DefaultMaterials[i]);
			
			for (int i = 0; i < old.ElementMaterials.Length; i++)
			{
				newApp.ElementMaterials[i] = new SlimeAppearanceMaterials
				{
					OverrideDefaults = old.ElementMaterials[i].OverrideDefaults,
					Materials = new Material[old.ElementMaterials[i].Materials.Length]
				};

				for (int m = 0; m < old.ElementMaterials[i].Materials.Length; m++)
				{
					newApp.ElementMaterials[i].Materials[m] = Object.Instantiate(old.ElementMaterials[i].Materials[m]);
				}
			}

			return newApp;
		}
	}
}
