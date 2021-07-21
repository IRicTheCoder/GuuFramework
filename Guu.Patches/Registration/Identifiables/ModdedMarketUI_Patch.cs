using Eden.Patching.Harmony;
using Guu.API;
using Guu.Game;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(ModdedMarketUI))]
	[UsedImplicitly]
	internal static class ModdedMarketUI_Patch
	{
		[UsedImplicitly]
		private static void Start_Prefix(ModdedMarketUI @this) => @this.plorts = AmmoRegistry.InjectMarketPlorts(@this.plorts);
	}
}