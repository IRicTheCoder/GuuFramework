using System;
using System.Collections.Generic;
using Guu.Game.General;
using Guu.Loader;
using JetBrains.Annotations;

namespace Guu.API
{
    /// <summary>The registry to register all ammo related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class AmmoRegistry
    {
        //+ DELEGATES
        /// <summary>Represents a lock to check if the player can hold an item</summary>
        public delegate bool CanPlayerHold(Identifiable.Id id, PlayerState.AmmoMode mode);

        /// <summary>Represents a lock to check if the storage can hold an item</summary>
        public delegate bool CanStorageHold(Identifiable.Id id, SiloStorage storage);
        
        /// <summary>Represents a lock to check if the catcher can receive an item</summary>
        public delegate bool CanCatcherReceive(Identifiable.Id id, SiloCatcher catcher);

        /// <summary>Identifies the max amount of ammo for a specific item or slot</summary>
        public delegate int? MaxAmmo(Identifiable.Id id, int slot, PlayerState.AmmoMode mode);
        
        //+ VARIABLES
        //? Player Related
        private static readonly Dictionary<PlayerState.AmmoMode, HashSet<Identifiable.Id>> INV_AMMO = new Dictionary<PlayerState.AmmoMode, HashSet<Identifiable.Id>>(PlayerState.AmmoModeComparer.Instance);
        private static readonly Dictionary<PlayerState.AmmoMode, Ammo> NEW_AMMO_MODES = new Dictionary<PlayerState.AmmoMode, Ammo>(PlayerState.AmmoModeComparer.Instance);
        private static readonly Dictionary<Identifiable.Id, List<InventoryLocker>> INV_LOCKS = new Dictionary<Identifiable.Id, List<InventoryLocker>>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, MaxAmmo> MAX_AMMO_IDENT = new Dictionary<Identifiable.Id, MaxAmmo>(Identifiable.idComparer);
        private static readonly Dictionary<int, MaxAmmo> MAX_AMMO_SLOT = new Dictionary<int, MaxAmmo>();
        
        //? Storage Related
        private static readonly Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>> STORAGE_AMMO = new Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>>(SiloStorage.StorageTypeComparer.Instance);
        private static readonly Dictionary<Identifiable.Id, List<StorageLocker>> STORAGE_LOCKS = new Dictionary<Identifiable.Id, List<StorageLocker>>(Identifiable.idComparer);
        
        //? Catcher Related
        private static readonly HashSet<Identifiable.Id> REFINERY_RESOURCES = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, List<CatcherLocker>> CATCHER_LOCKS = new Dictionary<Identifiable.Id, List<CatcherLocker>>(Identifiable.idComparer);
        
        //? Market Related
        private static readonly HashSet<MarketUI.PlortEntry> MARKET_PLORTS = new HashSet<MarketUI.PlortEntry>();
        private static readonly HashSet<EconomyDirector.ValueMap> MARKET_VALUES = new HashSet<EconomyDirector.ValueMap>();
        
        //? Registry Related
        private static readonly HashSet<AmmoHandler> HANDLERS = new HashSet<AmmoHandler> { new AmmoHandler().Setup() };
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new ammo for the player's inventory
        /// </summary>
        /// <param name="mode">The mode to register to</param>
        /// <param name="id">The ID to register</param>
        public static void RegisterPlayerAmmo(PlayerState.AmmoMode mode, Identifiable.Id id)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (!INV_AMMO.ContainsKey(mode))
                INV_AMMO.Add(mode, new HashSet<Identifiable.Id>(Identifiable.idComparer));

            INV_AMMO[mode].Add(id);
        }
        
        /// <summary>
        /// Registers a new ammo modes for the player
        /// </summary>
        /// <param name="mode">The mode to register</param>
        /// <param name="ammo">The ammo holder to contain the ammo for the mode</param>
        public static void RegisterAmmoMode(PlayerState.AmmoMode mode, Ammo ammo)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");

            NEW_AMMO_MODES[mode] = ammo;
        }

        /// <summary>
        /// Registers a locker for an Identifiable for the player inventory
        /// </summary>
        /// <param name="id">The ID of the identifiable</param>
        /// <param name="locker">The inventory locker</param>
        public static void RegisterInventoryLocker(Identifiable.Id id, InventoryLocker locker)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (!INV_LOCKS.ContainsKey(id))
                INV_LOCKS.Add(id, new List<InventoryLocker>());
            
            INV_LOCKS[id].Add(locker);
        }
        
        /// <summary>
        /// Registers a max ammo check for an Identifiable
        /// </summary>
        /// <param name="id">The ID of the identifiable</param>
        /// <param name="getter">The action to get the ammo</param>
        public static void RegisterMaxAmmo(Identifiable.Id id, MaxAmmo getter)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            MAX_AMMO_IDENT[id] = getter;
        }
        
        /// <summary>
        /// Registers a max ammo check for an Identifiable
        /// </summary>
        /// <param name="slot">The slot in the inventory</param>
        /// <param name="getter">The action to get the ammo</param>
        public static void RegisterMaxAmmo(int slot, MaxAmmo getter)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            MAX_AMMO_SLOT[slot] = getter;
        }

        /// <summary>
        /// Registers a new ammo for the storage inventories
        /// </summary>
        /// <param name="type">The type of storage to register to</param>
        /// <param name="id">The id to register</param>
        public static void RegisterStorageAmmo(SiloStorage.StorageType type, Identifiable.Id id)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (!STORAGE_AMMO.ContainsKey(type))
                STORAGE_AMMO.Add(type, new HashSet<Identifiable.Id>(Identifiable.idComparer));

            STORAGE_AMMO[type].Add(id);
        }
        
        /// <summary>
        /// Registers a locker for an Identifiable for the storage inventory
        /// </summary>
        /// <param name="id">The ID of the identifiable</param>
        /// <param name="locker">The storage locker</param>
        public static void RegisterStorageLocker(Identifiable.Id id, StorageLocker locker)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (!STORAGE_LOCKS.ContainsKey(id))
                STORAGE_LOCKS.Add(id, new List<StorageLocker>());
            
            STORAGE_LOCKS[id].Add(locker);
        }
        
        /// <summary>
        /// Registers a resource as part of the refinery
        /// </summary>
        /// <param name="id">The id to register</param>
        public static void RegisterRefineryResource(Identifiable.Id id)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            REFINERY_RESOURCES.Add(id);
        }
        
        /// <summary>
        /// Registers a locker for an Identifiable for a catcher
        /// </summary>
        /// <param name="id">The ID of the identifiable</param>
        /// <param name="locker">The catcher locker</param>
        public static void RegisterCatcherLocker(Identifiable.Id id, CatcherLocker locker)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (!CATCHER_LOCKS.ContainsKey(id))
                CATCHER_LOCKS.Add(id, new List<CatcherLocker>());
            
            CATCHER_LOCKS[id].Add(locker);
        }

        /// <summary>
        /// Registers a plort into the market
        /// </summary>
        /// <param name="entry">The entry to register</param>
        public static void RegisterMarketPlort(MarketUI.PlortEntry entry)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            MARKET_PLORTS.RemoveWhere(plortEntry => plortEntry.id == entry.id);
            MARKET_PLORTS.Add(entry);
        }

        /// <summary>
        /// Registers the price for a plort into the economy director
        /// </summary>
        /// <param name="valueMap">The map with the price value</param>
        public static void RegisterMarketPrice(EconomyDirector.ValueMap valueMap)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            MARKET_VALUES.RemoveWhere(map => map.accept == valueMap.accept);
            MARKET_VALUES.Add(valueMap);
        }
        
        /// <summary>
        /// Registers a new ammo handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(AmmoHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }
        
        //+ REMOVAL
        /// <summary>
        /// Removes an inventory locker from the registry
        /// </summary>
        /// <param name="id">The ID of the identifiable that has the locker</param>
        /// <param name="predicate">The predicate to find what it needs to remove</param>
        public static void RemoveInventoryLocker(Identifiable.Id id, Predicate<InventoryLocker> predicate)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only remove things from registry during the 'Register' step");
            
            if (!INV_LOCKS.ContainsKey(id)) return;
            INV_LOCKS[id].RemoveAll(predicate);
        }
        
        /// <summary>
        /// Removes a storage locker from the registry
        /// </summary>
        /// <param name="id">The ID of the identifiable that has the locker</param>
        /// <param name="predicate">The predicate to find what it needs to remove</param>
        public static void RemoveStorageLocker(Identifiable.Id id, Predicate<StorageLocker> predicate)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only remove things from registry during the 'Register' step");
            
            if (!STORAGE_LOCKS.ContainsKey(id)) return;
            STORAGE_LOCKS[id].RemoveAll(predicate);
        }
        
        /// <summary>
        /// Removes a catcher locker from the registry
        /// </summary>
        /// <param name="id">The ID of the identifiable that has the locker</param>
        /// <param name="predicate">The predicate to find what it needs to remove</param>
        public static void RemoveCatcherLocker(Identifiable.Id id, Predicate<CatcherLocker> predicate)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only remove things from registry during the 'Register' step");
            
            if (!CATCHER_LOCKS.ContainsKey(id)) return;
            CATCHER_LOCKS[id].RemoveAll(predicate);
        }
        
        //+ LOCKERS
        /// <summary>A locker for the player's inventory</summary>
        [UsedImplicitly]
        public class InventoryLocker : LockerBase<CanPlayerHold>
        {
            /// <inheritdoc />
            public InventoryLocker(CanPlayerHold check) : base(check) { }

            /// <summary>
            /// Checks if the upgrade is unlocked
            /// </summary>
            /// <param name="id">The ID to check</param>
            /// <param name="mode">The current ammo mode</param>
            /// <returns>True if unlocked, false otherwise</returns>
            public bool IsUnlocked(Identifiable.Id id, PlayerState.AmmoMode mode)
            {
                if (unlocked) return true;

                unlocked = lockerCheck?.Invoke(id, mode) ?? true;
                return unlocked;
            }
        }

        /// <summary>A locker for the storage inventory</summary>
        [UsedImplicitly]
        public class StorageLocker : LockerBase<CanStorageHold>
        {
            /// <inheritdoc />
            public StorageLocker(CanStorageHold check) : base(check) { }

            /// <summary>
            /// Checks if the upgrade is unlocked
            /// </summary>
            /// <param name="id">The ID to check</param>
            /// <param name="storage">The storage itself</param>
            /// <returns>True if unlocked, false otherwise</returns>
            public bool IsUnlocked(Identifiable.Id id, SiloStorage storage)
            {
                if (unlocked) return true;

                unlocked = lockerCheck?.Invoke(id, storage) ?? true;
                return unlocked;
            }
        }
        
        /// <summary>A locker for the catcher to check if it can receive an item</summary>
        [UsedImplicitly]
        public class CatcherLocker : LockerBase<CanCatcherReceive>
        {
            /// <inheritdoc />
            public CatcherLocker(CanCatcherReceive check) : base(check) { }

            /// <summary>
            /// Checks if the upgrade is unlocked
            /// </summary>
            /// <param name="id">The ID to check</param>
            /// <param name="catcher">The catcher itself</param>
            /// <returns>True if unlocked, false otherwise</returns>
            public bool IsUnlocked(Identifiable.Id id, SiloCatcher catcher)
            {
                if (unlocked) return true;

                unlocked = lockerCheck?.Invoke(id, catcher) ?? true;
                return unlocked;
            }
        }
    }
}