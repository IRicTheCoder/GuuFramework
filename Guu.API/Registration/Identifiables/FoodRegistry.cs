using System.Collections.Generic;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all food related things
    /// </summary>
    public static class FoodRegistry
    {
        //+ DELEGATE
        public delegate bool CanPlant(GardenCatcher catcher);
        
        //+ VARIABLES
        private static readonly Dictionary<Identifiable.Id, GardenCatcher.PlantSlot> PLANTABLES = new Dictionary<Identifiable.Id, GardenCatcher.PlantSlot>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, CanPlant> GARDEN_LOCKS = new Dictionary<Identifiable.Id, CanPlant>(Identifiable.idComparer);
    }
}