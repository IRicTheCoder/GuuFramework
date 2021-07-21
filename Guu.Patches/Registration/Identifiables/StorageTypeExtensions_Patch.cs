using System.Collections.Generic;
using System.Reflection;
using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

// ReSharper disable RedundantAssignment
// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(StorageTypeExtensions))]
	[UsedImplicitly]
	internal static class StorageTypeExtensions_Patch
	{
		[UsedImplicitly]
		private static bool GetContents_Prefix(SiloStorage.StorageType type, ref HashSet<Identifiable.Id> @return)
		{
			FieldInfo cacheField = typeof(StorageTypeExtensions).GetField("getContentsCache", BindingFlags.Static | BindingFlags.NonPublic);
			Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>> cache = cacheField?.GetValue(null) as Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>>;

			if (cache != null && cache.ContainsKey(type))
			{
				@return = cache[type];
				return false;
			}

			HashSet<Identifiable.Id> idSet = new HashSet<Identifiable.Id>(Identifiable.idComparer);
			switch (type)
			{
				case SiloStorage.StorageType.NON_SLIMES:
					IdentifiableRegistry.PopulateHashSet(idSet, IdentifiableType.NON_SLIME, IdentifiableType.ORNAMENT, IdentifiableType.ECHO, IdentifiableType.ECHO_NOTE);
					break;
				case SiloStorage.StorageType.PLORT:
					IdentifiableRegistry.PopulateHashSet(idSet, IdentifiableType.PLORT);
					break;
				case SiloStorage.StorageType.FOOD:
					IdentifiableRegistry.PopulateHashSet(idSet, IdentifiableType.FOOD, IdentifiableType.CHICK);
					break;
				case SiloStorage.StorageType.CRAFTING:
					IdentifiableRegistry.PopulateHashSet(idSet, IdentifiableType.PLORT, IdentifiableType.CRAFT);
					break;
				case SiloStorage.StorageType.ELDER:
					IdentifiableRegistry.PopulateHashSet(idSet, IdentifiableType.ELDER);
					break;
			}
			
			idSet.Remove(Identifiable.Id.QUICKSILVER_PLORT);
			AmmoRegistry.InjectStorageAmmo(type, idSet);
			
			cache?.Add(type, idSet);
			cacheField?.SetValue(null, cache);
			@return = idSet;
			return false;
		}
	}
}