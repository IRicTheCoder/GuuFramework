using Eden.Patching.Harmony;
using Guu.Loader;
using JetBrains.Annotations;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(SceneContext))]
	[UsedImplicitly]
	internal static class SceneContext_Patch
	{
		[UsedImplicitly]
		private static void Start_Postfix()
		{
			
		}
	}
}