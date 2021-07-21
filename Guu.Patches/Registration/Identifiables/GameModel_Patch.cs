using System.Collections.Generic;
using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(GameModel)), UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal static class GameModel_Patch
	{
		private static void RegisterActor_Postfix(long actorId, GameObject gameObj, Dictionary<long, ActorModel> p_actors)
		{
			Identifiable.Id id = Identifiable.GetId(gameObj);
			if (!IdentifiableRegistry.IsIdentifiableRegistered(id)) return;
			
			IDRegistry.RegisterActor(actorId);
		}
	}
}