using System.Reflection;
using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

// ReSharper disable RedundantAssignment
// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(SiloStorage))]
	[UsedImplicitly]
	internal static class SiloStorage_Patch
	{
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Identifiable.Id))]
		private static bool MaybeAddIdentifiable_Prefix(Identifiable.Id id, SiloStorage @this, ref bool @return)
		{
			@return = AmmoRegistry.CheckStorageLocks(id, @this) && @this.GetRelevantAmmo().MaybeAddToSlot(id, null);
			@this.GetType().GetMethod("OnAdded", BindingFlags.NonPublic)?.Invoke(@this, new object[0]);
			return false;
		}
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Identifiable.Id), typeof(int), typeof(int), typeof(bool))]
		private static bool MaybeAddIdentifiable_Prefix1(Identifiable.Id id, int slotIdx, int count, bool overflow, SiloStorage @this, ref bool @return)
		{
			@return = AmmoRegistry.CheckStorageLocks(id, @this) && @this.GetRelevantAmmo().MaybeAddToSpecificSlot(id, null, slotIdx, count, overflow);
			@this.GetType().GetMethod("OnAdded", BindingFlags.NonPublic)?.Invoke(@this, new object[0]);
			return false;
		}
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Identifiable.Id))]
		private static bool CanAccept_Prefix(Identifiable.Id id, SiloStorage @this, ref bool @return)
		{
			@return = AmmoRegistry.CheckStorageLocks(id, @this) && @this.GetRelevantAmmo().CouldAddToSlot(id);
			return false;
		}
		
		[UsedImplicitly]
		[EdenHarmony.DefineOriginal(typeof(Identifiable.Id), typeof(int), typeof(bool))]
		private static bool CanAccept_Prefix(Identifiable.Id id, int slotIdx, bool overflow, SiloStorage @this, ref bool @return)
		{
			@return = AmmoRegistry.CheckStorageLocks(id, @this) && @this.GetRelevantAmmo().CouldAddToSlot(id, slotIdx, overflow);
			return false;
		}
	}
}