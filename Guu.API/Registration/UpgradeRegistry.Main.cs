using System;
using System.Collections.Generic;
using Guu.Loader;
using JetBrains.Annotations;

namespace Guu.API
{
    /// <summary>The registry to register all upgrade related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class UpgradeRegistry
    {
        //+ VARIABLES
        //? Player Related
        private static readonly Dictionary<PlayerState.Upgrade, bool> PLAYER_UPGRADES = new Dictionary<PlayerState.Upgrade, bool>(PlayerState.upgradeComparer);
        private static readonly Dictionary<PlayerState.Upgrade, Tuple<PlayerState.UnlockCondition, float>> PLAYER_UPGRADE_LOCKS = new Dictionary<PlayerState.Upgrade, Tuple<PlayerState.UnlockCondition, float>>(PlayerState.upgradeComparer);
        
        //? Registry Related
        private static readonly HashSet<UpgradeHandler> HANDLERS = new HashSet<UpgradeHandler> { new UpgradeHandler().Setup() };
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new upgrade for te player
        /// </summary>
        /// <param name="upgrade">The upgrade to register</param>
        /// <param name="startAvailable">Should the upgrade start available?</param>
        /// <param name="startUnlocked">Should the upgrade start unlocked?</param>
        public static void RegisterPlayerUpgrade(PlayerState.Upgrade upgrade, bool startAvailable, bool startUnlocked = false)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");

            if ((startAvailable || startUnlocked) && !PLAYER_UPGRADE_LOCKS.ContainsKey(upgrade))
                PLAYER_UPGRADES[upgrade] = startUnlocked;
        }

        /// <summary>
        /// Registers the lock for a player upgrade
        /// </summary>
        /// <param name="upgrade">The upgrade to register the lock for</param>
        /// <param name="unlockCondition">The unlock condition</param>
        /// <param name="unlockDelayHrs">The amount of game hours to delay</param>
        public static void RegisterPlayerUpgradeLock(PlayerState.Upgrade upgrade, PlayerState.UnlockCondition unlockCondition, float unlockDelayHrs)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            if (PLAYER_UPGRADES.ContainsKey(upgrade)) PLAYER_UPGRADES.Remove(upgrade);
            PLAYER_UPGRADE_LOCKS[upgrade] = new Tuple<PlayerState.UnlockCondition, float>(unlockCondition, unlockDelayHrs);
        }
        
        /// <summary>
        /// Registers a new upgrade handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(UpgradeHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }
    }
}