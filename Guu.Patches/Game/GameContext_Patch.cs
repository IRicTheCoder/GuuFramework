using Eden.Patching.Harmony;
using Guu.Loader;
using JetBrains.Annotations;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(GameContext))]
	[UsedImplicitly]
	internal static class GameContext_Patch
	{
		[UsedImplicitly]
		private static void Start_Prefix()
		{
			ModLoader.Load();
			ModLoader.Handle();
		}
		
		[UsedImplicitly]
		private static void Start_Postfix()
		{
			ModLoader.PostLoad();
			ModLoader.Comms();
			
			//SRGuu.OnGameContextReady(__instance);
			//GameUtils.AddComponentToIdentifiable<ModdedData>();
		}
	}
}