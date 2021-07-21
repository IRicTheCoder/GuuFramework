using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all ammo related things</summary>
	public static partial class AmmoRegistry
	{
        //+ VANILLA REFERENCES
        private static readonly List<SiloStorage.StorageType> VANILLA_STORAGE_TYPE = new List<SiloStorage.StorageType>()
        {
            SiloStorage.StorageType.FOOD,
            SiloStorage.StorageType.ELDER,
            SiloStorage.StorageType.PLORT,
            SiloStorage.StorageType.CRAFTING,
            SiloStorage.StorageType.NON_SLIMES
        };
        
        //+ HANDLING
        internal static void InjectAmmoModes(PlayerModel model, Dictionary<PlayerState.AmmoMode, Ammo> ammoDict)
        {
            foreach (PlayerState.AmmoMode mode in NEW_AMMO_MODES.Keys)
            {
                model.ammoDict[mode] = new AmmoModel();
                ammoDict[mode] = NEW_AMMO_MODES[mode];
                ammoDict[mode].SetModel(model.ammoDict[mode]);
            }
        }

        internal static void LoadAmmoModels(PlayerModel model, Dictionary<PlayerState.AmmoMode, Ammo> ammoDict)
        {
            foreach (PlayerState.AmmoMode mode in NEW_AMMO_MODES.Keys)
                ammoDict[mode].SetModel(model.ammoDict[mode]);
        }
        
        internal static void InjectInvAmmo(Dictionary<PlayerState.AmmoMode, Ammo> ammoDict)
        {
            foreach (PlayerState.AmmoMode mode in INV_AMMO.Keys)
            {
                if (!ammoDict.ContainsKey(mode)) continue;

                foreach (Identifiable.Id id in INV_AMMO[mode])
                    ammoDict[mode].RegisterPotentialAmmo(GameContext.Instance.LookupDirector.GetPrefab(id));
            }
        }

        internal static int? RetrieveMaxAmmo(Identifiable.Id id, int slot, PlayerState.AmmoMode mode)
        {
            int? result = null;
            
            if (MAX_AMMO_IDENT.ContainsKey(id)) result = MAX_AMMO_IDENT[id].Invoke(id, slot, mode);
            if (result == null && MAX_AMMO_SLOT.ContainsKey(slot)) result = MAX_AMMO_SLOT[slot].Invoke(id, slot, mode);

            return result;
        }

        internal static void InjectStorageAmmo(SiloStorage.StorageType type, HashSet<Identifiable.Id> idSet)
        {
            if (!STORAGE_AMMO.ContainsKey(type)) return;
            idSet.UnionWith(STORAGE_AMMO[type]);
        }
        
        internal static EconomyDirector.ValueMap[] InjectMarketPrices(EconomyDirector.ValueMap[] baseValueMap) => baseValueMap.Concat(MARKET_VALUES).ToArray();
        internal static MarketUI.PlortEntry[] InjectMarketPlorts(MarketUI.PlortEntry[] plorts) => plorts.Concat(MARKET_PLORTS).ToArray();
        internal static Identifiable.Id[] InjectRefineryResources(Identifiable.Id[] listedItems) => listedItems.Concat(REFINERY_RESOURCES).ToArray();
        internal static bool CheckRefineryResource(Identifiable.Id id) => REFINERY_RESOURCES.Contains(id);

        internal static bool CheckInventoryLocks(Identifiable.Id id, PlayerState.AmmoMode mode)
        {
            if (!INV_LOCKS.ContainsKey(id)) return true;

            foreach (InventoryLocker locker in INV_LOCKS[id])
                if (!locker.IsUnlocked(id, mode)) return false;

            return true;
        }
        
        internal static bool CheckStorageLocks(Identifiable.Id id, SiloStorage storage)
        {
            if (!STORAGE_LOCKS.ContainsKey(id)) return true;

            foreach (StorageLocker locker in STORAGE_LOCKS[id])
                if (!locker.IsUnlocked(id, storage)) return false;

            return true;
        }
        
        internal static bool CheckCatcherLocks(Identifiable.Id id, SiloCatcher catcher)
        {
            if (!STORAGE_LOCKS.ContainsKey(id)) return true;

            foreach (CatcherLocker locker in CATCHER_LOCKS[id])
                if (!locker.IsUnlocked(id, catcher)) return false;

            return true;
        }
        
        //+ SAVE HANDLING
        internal static bool IsAmmoModeRegistered(PlayerState.AmmoMode mode) => NEW_AMMO_MODES.ContainsKey(mode);
        internal static bool IsStorageTypeRegistered(SiloStorage.StorageType type) => !VANILLA_STORAGE_TYPE.Contains(type) && STORAGE_AMMO.ContainsKey(type);
    }
}