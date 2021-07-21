using System.Collections.Generic;
using Guu.Game;

namespace Guu.API
{
    /// <summary>
    /// Serves as a handler for resources after being registered, mostly to sort them
    /// and identify them. Make a child of this class to create your own handlers
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AmmoHandler : IHandler<AmmoHandler>
    {
        //+ HANDLING
        /// <inheritdoc />
        public virtual AmmoHandler Setup()
        {
            APIHandler.HandleRegistration += RegistryHandle;
            APIHandler.HandleItems += Handle;
            APIHandler.ClearMemory += ClearMemory;
            
            return this;
        }
        
        /// <inheritdoc />
        public virtual void RegistryHandle() { }

        /// <inheritdoc />
        public virtual void Handle() { }

        /// <inheritdoc />
        public virtual void ClearMemory() { }
        
        //+ LOCK CHECK
        /// <summary>Checks if a player can hold a specific item in a specific ammo mode</summary>
        public static bool CanPlayerHold(Identifiable.Id id, PlayerState.AmmoMode mode) => AmmoRegistry.CheckInventoryLocks(id, mode);
        
        /// <summary>Checks if a storage can hold a specific item</summary>
        public static bool CanStorageHold(Identifiable.Id id, SiloStorage storage) => AmmoRegistry.CheckStorageLocks(id, storage);
        
        /// <summary>Checks if a catcher can receive a specific item</summary>
        public static bool CanCatcherReceive(Identifiable.Id id, SiloCatcher catcher) => AmmoRegistry.CheckCatcherLocks(id, catcher);

        /// <summary>Retrieves the max ammo for an identifiable or inventory slot</summary>
        public static int GetMaxAmmo(Identifiable.Id id, int slot, PlayerState.AmmoMode mode) => AmmoRegistry.RetrieveMaxAmmo(id, slot, mode) ?? -1;

        //+ VERIFICATION
        /// <summary>
        /// Checks the amount of plorts collected of a certain id
        /// </summary>
        /// <param name="id">The ID of the plort to check</param>
        /// <returns>The amount of plorts collected</returns>
        public static int PlortsCollected(Identifiable.Id id) => ModdedMarketUI.PlortsCollected(id);
    }
}