using Eden.Patching.Harmony;
using Guu.Loader;
using JetBrains.Annotations;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(AnalyticsUtil))]
	[UsedImplicitly]
	internal static class AnalyticsUtil_Patch
	{
		[UsedImplicitly]
		private static bool CustomEvent_Prefix() => false;
	}
}