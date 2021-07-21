using System.Collections.Generic;
using JetBrains.Annotations;

namespace Guu.API
{
    /// <summary>The registry to register all food related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [System.Obsolete("NOT IMPLEMENTED YET")]
    public static class FoodRegistry
    {
        //+ DELEGATE
        /// <summary>A delegate to check if the given garden catcher can accept the item being planted</summary>
        public delegate bool CanPlant(GardenCatcher catcher);
        
        //+ VARIABLES
        private static readonly Dictionary<Identifiable.Id, GardenCatcher.PlantSlot> PLANTABLES = new Dictionary<Identifiable.Id, GardenCatcher.PlantSlot>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, CanPlant> GARDEN_LOCKS = new Dictionary<Identifiable.Id, CanPlant>(Identifiable.idComparer);
        
        private static readonly HashSet<FoodHandler> HANDLERS = new HashSet<FoodHandler> { new FoodHandler().InternalSetup() };
        
        private static readonly Dictionary<SlimeEat.FoodGroup, HashSet<Identifiable.Id>> FOOD_GROUPS = new Dictionary<SlimeEat.FoodGroup, HashSet<Identifiable.Id>>()
        {
            { SlimeEat.FoodGroup.MEAT, FoodHandler.MEAT_GROUP },
            { SlimeEat.FoodGroup.FRUIT, FoodHandler.FRUIT_GROUP },
            { SlimeEat.FoodGroup.GINGER, FoodHandler.GINGER_GROUP },
            { SlimeEat.FoodGroup.PLORTS, FoodHandler.PLORTS_GROUP },
            { SlimeEat.FoodGroup.VEGGIES, FoodHandler.VEGGIES_GROUP },
            { SlimeEat.FoodGroup.NONTARRGOLD_SLIMES, FoodHandler.NONTARRGOLD_SLIMES_GROUP }
        };
        
        //+ REGISTRATION
        public static void RegisterPlantable(GardenCatcher.PlantSlot plantSlot)
        {
            /*if (PLANTABLES.ContainsKey(plantSlot.id)) PLANTABLES[plantSlot.id] = plantSlot;
            else PLANTABLES.Add(plantSlot.id, plantSlot);*/
        }

        public static void RegisterGardenLocks(Identifiable.Id id, CanPlant canPlant)
        {
            /*if (GARDEN_LOCKS.ContainsKey(id)) GARDEN_LOCKS[id] = canPlant;
            else GARDEN_LOCKS.Add(id, canPlant);*/
        }

        public static void RegisterFoodGroup(SlimeEat.FoodGroup group, HashSet<Identifiable.Id> hashSet)
        {
            /*if (!FOOD_GROUPS.ContainsKey(group))
            {
                GuuCore.LOGGER.LogError($"The food group {group} is already registered");
                return;
            }
            
            FOOD_GROUPS.Add(group, hashSet);*/
        }
    }
}