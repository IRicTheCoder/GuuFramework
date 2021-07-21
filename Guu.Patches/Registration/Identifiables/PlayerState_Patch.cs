using System.Collections.Generic;
using Eden.Patching.Harmony;
using Guu.API;
using HarmonyLib;
using JetBrains.Annotations;
using MonomiPark.SlimeRancher.DataModel;

// ReSharper disable RedundantAssignment
// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
	[EdenHarmony.Wrapper(typeof(PlayerState))]
	[UsedImplicitly]
	internal static class PlayerState_Patch
	{
		[UsedImplicitly]
		private static void SetModel_Prefix(PlayerModel model, Dictionary<PlayerState.AmmoMode, Ammo> p_ammoDict) => AmmoRegistry.LoadAmmoModels(model, p_ammoDict);

		[UsedImplicitly]
		private static void Reset_Postfix(PlayerModel model, Dictionary<PlayerState.AmmoMode, Ammo> p_ammoDict)
		{
			//& Add the new ammo's into the game
			AmmoRegistry.InjectAmmoModes(model, p_ammoDict);
			AmmoRegistry.InjectInvAmmo(p_ammoDict);
		}

		[UsedImplicitly] 
		private static bool GetMaxAmmo_Default_Prefix(Identifiable.Id id, int index, ref int @return, PlayerState @this)
		{
			int? value = AmmoRegistry.RetrieveMaxAmmo(id, index, @this.GetAmmoMode());
			@return = value ?? -1;

			return value == null;
		}

		[UsedImplicitly]
		private static bool GetMaxAmmo_NimbleValley_Prefix(Identifiable.Id id, int index, ref int @return, PlayerState @this)
		{
			switch (index)
			{
				case 0:
					@return = 250;
					break;
				case 1:
					@return = 100;
					break;
				case 2:
					@return = 3;
					break;
				default:
					@return = 0;
					break;
			}
			
			@return = AmmoRegistry.RetrieveMaxAmmo(id, index, @this.GetAmmoMode()) ?? @return;
			return false;
		}
	}
}