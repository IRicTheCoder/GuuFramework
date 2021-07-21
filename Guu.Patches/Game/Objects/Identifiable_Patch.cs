using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(Identifiable))]
	[UsedImplicitly]
	internal static class Identifiable_Patch
	{
		[UsedImplicitly]
		private static bool IsAnimal_Prefix(out bool @return, Identifiable.Id id)
		{
			@return = IdentifiableHandler.IsAnimal(id);
			return false;
		}

		[UsedImplicitly]
		private static bool IsWater_Prefix(out bool @return, Identifiable.Id id)
		{
			@return = IdentifiableHandler.IsWater(id);
			return false;
		}
	}
}