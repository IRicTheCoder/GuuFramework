using System.Collections.Generic;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all ammo related things
    /// </summary>
    public static class AmmoRegistry
    {
        //+ VARIABLES
        private static readonly Dictionary<PlayerState.AmmoMode, HashSet<Identifiable.Id>> INV_AMMO = new Dictionary<PlayerState.AmmoMode, HashSet<Identifiable.Id>>();
        private static readonly Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>> STORAGE_AMMO = new Dictionary<SiloStorage.StorageType, HashSet<Identifiable.Id>>();
    }
}