using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(GadgetDirector))]
	[UsedImplicitly]
	internal static class GadgetDirector_Patch
	{
		[UsedImplicitly]
		private static bool IsRefineryResource_Prefix(Identifiable.Id id, out bool @return)
		{
			@return = IdentifiableHandler.IsPlort(id) || IdentifiableHandler.IsCraft(id) || AmmoRegistry.CheckRefineryResource(id);
			return false;
		}
	}
}