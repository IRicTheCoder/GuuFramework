using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(EconomyDirector))]
	[UsedImplicitly]
	internal static class EconomyDirector_Patch
	{
		[UsedImplicitly]
		private static void InitModel_Prefix(EconomyDirector @this) => @this.baseValueMap = AmmoRegistry.InjectMarketPrices(@this.baseValueMap);
	}
}