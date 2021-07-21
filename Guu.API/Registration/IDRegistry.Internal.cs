using System.Linq;
using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all id related things</summary>
    public static partial class IDRegistry
    {
		//+ REGISTRATION
		internal static void RegisterActor(long actorID) => ACTOR_IDS.Add(actorID);

		//+ HANDLING

		//+ SAVE HANDLING
		internal static bool IsActorRegistered(long actorID) => ACTOR_IDS.Contains(actorID);
	}
}