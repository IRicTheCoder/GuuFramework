using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(RefineryUI))]
	[UsedImplicitly]
	internal static class RefineryUI_Patch
	{
		[UsedImplicitly]
		private static void Awake_Prefix(RefineryUI @this) => @this.listedItems = AmmoRegistry.InjectRefineryResources(@this.listedItems);
	}
}