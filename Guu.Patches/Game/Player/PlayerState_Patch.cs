using Eden.Patching.Harmony;
using JetBrains.Annotations;
using UnityEngine;
using EventHandler = Guu.Services.Events.EventHandler;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(PlayerState))]
	[UsedImplicitly]
	internal static class PlayerState_Patch
	{
		private static bool gadgetModeState;
		
		//+ BEHAVIOUR
		[UsedImplicitly]
		private static void Awake_Postfix(PlayerState @this) => @this.onEndGame += EventHandler.Instance.OnEndGame_Trigger;

		[UsedImplicitly]
		private static void Update_Postfix(PlayerState @this)
		{
			if (gadgetModeState == @this.InGadgetMode) return;
			
			@this.InGadgetMode = EventHandler.Instance.OnGadgetModeChanged_Trigger(@this, @this.InGadgetMode);
			gadgetModeState = @this.InGadgetMode;
		}
			
		//+ TRIGGERS
		[UsedImplicitly] private static void OnEnteredZone_Postfix(PlayerState @this, ZoneDirector.Zone zone) => EventHandler.Instance.OnEnteredZone_Trigger(@this, zone);
		[UsedImplicitly] private static void UnlockMap_Postfix(PlayerState @this, ZoneDirector.Zone zone) => EventHandler.Instance.OnUnlockZone_Trigger(@this, zone);
		[UsedImplicitly] private static void SetAmmoMode_Postfix(PlayerState @this, PlayerState.AmmoMode mode) => EventHandler.Instance.OnSetAmmoMode_Trigger(@this, mode);
		[UsedImplicitly] private static void SetEnergy_Postfix(PlayerState @this, int energy) => EventHandler.Instance.OnSetEnergy_Trigger(@this, energy);
		[UsedImplicitly] private static void SetRad_Postfix(PlayerState @this, int rad) => EventHandler.Instance.OnSetRad_Trigger(@this, rad);
		[UsedImplicitly] private static void SetHealth_Postfix(PlayerState @this, int health) => EventHandler.Instance.OnSetHealth_Trigger(@this, health);
		[UsedImplicitly] private static void Heal_Postfix(PlayerState @this, int healthGain) => EventHandler.Instance.OnHeal_Trigger(@this, healthGain);
		[UsedImplicitly] private static void AddCurrency_Postfix(PlayerState @this, int adjust, PlayerState.CoinsType coinsType) => EventHandler.Instance.OnAddCurrency_Trigger(@this, adjust, coinsType);
		[UsedImplicitly] private static void AddKey_Postfix(PlayerState @this) => EventHandler.Instance.OnAddKey_Trigger(@this);
		[UsedImplicitly] private static void SpendKey_Postfix(PlayerState @this) => EventHandler.Instance.OnSpendKey_Trigger(@this);
		[UsedImplicitly] private static void AddUpgrade_Postfix(PlayerState @this, PlayerState.Upgrade upgrade, bool isFirstTime) => EventHandler.Instance.OnAddUpgrade_Trigger(@this, upgrade, isFirstTime);
		[UsedImplicitly] private static void OnExitedZone_Postfix(PlayerState @this, ZoneDirector.Zone zone) => EventHandler.Instance.OnExitedZone_Trigger(@this, zone);

		[UsedImplicitly] private static bool AddRads_Prefix(PlayerState @this, float rads) => EventHandler.Instance.OnAddRads_Trigger(@this, rads);
		[UsedImplicitly] private static bool RemoveRads_Prefix(PlayerState @this, float rads) => EventHandler.Instance.OnRemoveRads_Trigger(@this, rads);
		[UsedImplicitly] private static bool Damage_Prefix(PlayerState @this, int healthLoss, GameObject source) => EventHandler.Instance.OnDamage_Trigger(@this, healthLoss, source);
		[UsedImplicitly] private static bool SpendEnergy_Prefix(PlayerState @this, float energy) => EventHandler.Instance.OnSpendEnergy_Trigger(@this, energy);
		[UsedImplicitly] private static bool SpendCurrency_Prefix(PlayerState @this, int adjust, bool forcedLoss) => EventHandler.Instance.OnSpendCurrency_Trigger(@this, adjust, forcedLoss);
	}
}