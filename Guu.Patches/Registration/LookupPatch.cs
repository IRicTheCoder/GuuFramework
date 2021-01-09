using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guu.API;
using HarmonyLib;

namespace Guu.Patches.Registration
{
    [HarmonyPatch(typeof (SlimeEat))]
    [HarmonyPatch("Awake")]
    internal static class LookupPatch
    {
        private static void Prefix(LookupDirector __instance)
        {
        }
        
        private static void Postfix(LookupDirector __instance)
        {
            //+ Injects main game content
            IdentifiableRegistry.InjectPrefabs(__instance.IdentPrefabs());
            GadgetRegistry.InjectGadgets(__instance.GadgetDefs());
            IdentifiableRegistry.InjectVacEntries(__instance.VacItemDefs());
            
            // TODO: Review the following code
            //+ Patch the Food Groups
            FieldInfo group = typeof(SlimeEat).GetField("foodGroupIds");
            Dictionary<SlimeEat.FoodGroup, Identifiable.Id[]> foodGroups =
                group.GetValue(null) as Dictionary<SlimeEat.FoodGroup, Identifiable.Id[]>;

            if (foodGroups == null) return;
            
            // Collects the default food groups into the registry
            foreach (SlimeEat.FoodGroup key in foodGroups.Keys)
            {
                FoodGroupRegistry.FOOD_GROUPS[key] = new HashSet<Identifiable.Id>(foodGroups[key], Identifiable.idComparer);
            }
            
            // Adds the new food groups into the registry
            foreach (SlimeEat.FoodGroup key in FoodGroupRegistry.FOOD_GROUPS.Keys)
            {
                if (foodGroups.ContainsKey(key)) continue;
                
                foodGroups.Add(key, FoodGroupRegistry.FOOD_GROUPS[key].ToArray());
            }
            
            // Sets the value back
            group.SetValue(null, foodGroups);
            
            //+ Runs all handlers
            IdentifiableRegistry.RunHandlers();
            GadgetRegistry.RunHandlers();
        }
    }
}