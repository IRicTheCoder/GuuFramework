using UnityEngine;

namespace Guu.Services.Events
{
    /// <summary>The service that allows handling of events</summary>
    public partial class EventHandler
    {
        //+ TRIGGERS
        //? Options
        internal void OnAudioLevelsChanged_Trigger(OptionsUI optionsUI) => OnAudioLevelsChanged?.Handle(args: new object[]{ optionsUI }, unique: true);
        internal void OnApplyResolution_Trigger(OptionsUI optionsUI) => OnApplyResolution?.Handle(args: new object[]{ optionsUI }, unique: true);
        
        //? Message Director
        internal void OnBundlesAvailable_Trigger(MessageDirector dir) => OnBundlesAvailable?.Handle(args: new object[]{ dir }, unique: true);
        
        //? Player State
        internal void OnEnteredZone_Trigger(PlayerState player, ZoneDirector.Zone zone) => OnEnteredZone?.Handle(args: new object[] { player, zone }, unique: true);
        internal void OnUnlockZone_Trigger(PlayerState player, ZoneDirector.Zone zone) => OnUnlockZone?.Handle(args: new object[] { player, zone }, unique: true);
        internal void OnSetAmmoMode_Trigger(PlayerState player, PlayerState.AmmoMode mode) => OnSetAmmoMode?.Handle(args: new object[] { player, mode }, unique: true);
        internal void OnSetEnergy_Trigger(PlayerState player, int energy) => OnSetEnergy?.Handle(args: new object[] { player, energy }, unique: true);
        internal void OnSetHealth_Trigger(PlayerState player, int health) => OnSetHealth?.Handle(args: new object[] { player, health }, unique: true);
        internal void OnSetRad_Trigger(PlayerState player, int rad) => OnSetRad?.Handle(args: new object[] { player, rad }, unique: true);
        internal bool OnAddRads_Trigger(PlayerState player, float rads) => OnAddRads?.Handle<bool>(args: new object[] { player, rads }, unique: true) ?? true;
        internal bool OnRemoveRads_Trigger(PlayerState player, float rads) => OnRemoveRads?.Handle<bool>(args: new object[] { player, rads }, unique: true) ?? true;
        internal bool OnDamage_Trigger(PlayerState player, int healthLoss, GameObject source) => OnDamage?.Handle<bool>(args: new object[] { player, healthLoss, source }, unique: true) ?? true;
        internal void OnHeal_Trigger(PlayerState player, int healthGain) => OnHeal?.Handle(args: new object[] { player, healthGain }, unique: true);
        internal bool OnSpendEnergy_Trigger(PlayerState player, float energy) => OnSpendEnergy?.Handle<bool>(args: new object[] { player, energy }, unique: true) ?? true;
        internal void OnAddCurrency_Trigger(PlayerState player, int currency, PlayerState.CoinsType type) => OnAddCurrency?.Handle(args: new object[] { player, currency, type }, unique: true);
        internal bool OnSpendCurrency_Trigger(PlayerState player, int currency, bool forcedLoss) => OnSpendCurrency?.Handle<bool>(args: new object[] { player, currency, forcedLoss }, unique: true) ?? true;
        internal void OnAddKey_Trigger(PlayerState player) => OnAddKey?.Handle(args: new object[] { player }, unique: true);
        internal void OnSpendKey_Trigger(PlayerState player) => OnSpendKey?.Handle(args: new object[] { player }, unique: true);
        internal void OnAddUpgrade_Trigger(PlayerState player, PlayerState.Upgrade upgrade, bool isFirstTime) => OnAddUpgrade?.Handle(args: new object[] { player, upgrade, isFirstTime }, unique: true);
        internal void OnExitedZone_Trigger(PlayerState player, ZoneDirector.Zone zone) => OnExitedZone?.Handle(args: new object[] { player, zone }, unique: true);
        internal void OnEndGame_Trigger() => OnEndGame?.Handle(args: new object[] { SR.PlayerState }, unique: true);
        internal bool OnGadgetModeChanged_Trigger(PlayerState player, bool gadgetMode) => OnGadgetModeChanged?.Handle<bool>(args: new object[] { player, gadgetMode }, unique: true) ?? gadgetMode;
    }
}