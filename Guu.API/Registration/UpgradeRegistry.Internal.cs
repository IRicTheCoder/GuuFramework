using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all upgrade related things</summary>
    public static partial class UpgradeRegistry
    {
        //+ HANDLING
        internal static void InjectPlayerUpgrades(PlayerState state, PlayerModel model)
        {
            model.upgrades.AddAll(PLAYER_UPGRADES.Keys, upgrade => PLAYER_UPGRADES[upgrade]);
            model.availUpgrades.AddAll(PLAYER_UPGRADES.Keys, upgrade => !PLAYER_UPGRADES[upgrade]);
            model.upgradeLocks.AddEach(PLAYER_UPGRADE_LOCKS.Keys, upgrade => new PlayerState.UpgradeLocker(state, PLAYER_UPGRADE_LOCKS[upgrade].Item1, PLAYER_UPGRADE_LOCKS[upgrade].Item2));
        }

        //+ SAVE HANDLING
        internal static bool IsPlayerUpgradeRegistered(PlayerState.Upgrade upgrade) => PLAYER_UPGRADES.ContainsKey(upgrade);
    }
}