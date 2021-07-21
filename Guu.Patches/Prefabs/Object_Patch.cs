using Eden.Patching.Harmony;
using Guu.API;
using Guu.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.Patches.Prefabs
{
	[UsedImplicitly]
	[EdenHarmony.Wrapper(typeof(Object))]
	internal static class Object_Patch
	{
		[UsedImplicitly]
		[EdenHarmony.DefineGeneric(typeof(Object))]
		private static void Instantiate_Postfix(ref Object @return) => FixInstantiation(ref @return);
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Object))]
		private static void Instantiate_Postfix1(ref Object @return) => FixInstantiation(ref @return);

		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Object), typeof(Transform))]
		private static void Instantiate_Postfix2(ref Object @return) => FixInstantiation(ref @return);
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Object), typeof(Transform), typeof(bool))]
		private static void Instantiate_Postfix3(ref Object @return) => FixInstantiation(ref @return);
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Object), typeof(Vector3), typeof(Quaternion))]
		private static void Instantiate_Postfix4(ref Object @return) => FixInstantiation(ref @return);
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Object), typeof(Vector3), typeof(Quaternion), typeof(Transform))]
		private static void Instantiate_Postfix5(ref Object @return) => FixInstantiation(ref @return);

		[EdenHarmony.Ignore]
		private static void FixInstantiation(ref Object obj)
		{
			if (!(obj is GameObject go)) return;
			if (go.HasComponent(out PrefabUtils.ModdedPrefab moddedPrefab))
			{
				moddedPrefab.gameObject.SetActive(true);
				Object.Destroy(moddedPrefab, 0.5f);
				return;
			}
			
			if (!go.HasComponent(out APIUtils.APIPrefab apiPrefab)) return;

			bool originState = apiPrefab.apiBuilder.BaseItem.activeSelf;
			apiPrefab.apiBuilder.BaseItem.SetActive(false);

			GameObject prefab = Object.Instantiate(apiPrefab.apiBuilder.BaseItem);
			prefab.transform.position = go.transform.position;
			prefab.transform.rotation = go.transform.rotation;
			apiPrefab.apiBuilder.Build(prefab);
			prefab.SetActive(true);
			
			apiPrefab.apiBuilder.BaseItem.SetActive(originState);
			obj = prefab;
			Object.Destroy(go, 0.5f);
		}
	}
}