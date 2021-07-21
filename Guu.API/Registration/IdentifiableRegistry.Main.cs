using System;
using System.Collections.Generic;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all identifiable related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class IdentifiableRegistry
    {
        //+ VARIABLES
        //? Identifiable Related
        private static readonly Dictionary<Identifiable.Id, VacItemDefinition> VAC_ITEM_DEFS = new Dictionary<Identifiable.Id, VacItemDefinition>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, GameObject> IDENTIFIABLES = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);

        //? Registry Related
        private static readonly HashSet<IdentifiableHandler> HANDLERS = new HashSet<IdentifiableHandler> { new IdentifiableHandler().InternalSetup().Setup() };
        
        private static readonly Dictionary<IdentifiableType, HashSet<Identifiable.Id>> CLASS_LISTING = new Dictionary<IdentifiableType, HashSet<Identifiable.Id>>
        {
            { IdentifiableType.VEGGIE, Identifiable.VEGGIE_CLASS },
            { IdentifiableType.FRUIT, Identifiable.FRUIT_CLASS },
            { IdentifiableType.MEAT, Identifiable.MEAT_CLASS },
            { IdentifiableType.TOFU, Identifiable.TOFU_CLASS },
            { IdentifiableType.SLIME, Identifiable.SLIME_CLASS },
            { IdentifiableType.LARGO, IdentifiableHandler.MODDED_LARGO_CLASS },
            { IdentifiableType.GORDO, Identifiable.GORDO_CLASS },
            { IdentifiableType.PLORT, Identifiable.PLORT_CLASS },
            { IdentifiableType.FOOD, Identifiable.FOOD_CLASS },
            { IdentifiableType.NON_SLIME, Identifiable.NON_SLIMES_CLASS },
            { IdentifiableType.CHICK, Identifiable.CHICK_CLASS },
            { IdentifiableType.LIQUID, Identifiable.LIQUID_CLASS },
            { IdentifiableType.CRAFT, Identifiable.CRAFT_CLASS },
            { IdentifiableType.FASHION, Identifiable.FASHION_CLASS },
            { IdentifiableType.ECHO, Identifiable.ECHO_CLASS },
            { IdentifiableType.ECHO_NOTE, Identifiable.ECHO_NOTE_CLASS },
            { IdentifiableType.ORNAMENT, Identifiable.ORNAMENT_CLASS },
            { IdentifiableType.TOY, Identifiable.TOY_CLASS },
            { IdentifiableType.EATERS, Identifiable.EATERS_CLASS },
            { IdentifiableType.ALLERGY_FREE, Identifiable.ALLERGY_FREE_CLASS },
            { IdentifiableType.STANDARD_CRATE, Identifiable.STANDARD_CRATE_CLASS },
            { IdentifiableType.ELDER, Identifiable.ELDER_CLASS },
            { IdentifiableType.TARR, Identifiable.TARR_CLASS },
            { IdentifiableType.BOOP, Identifiable.BOOP_CLASS },
            { IdentifiableType.SCENE_OBJECT, Identifiable.SCENE_OBJECTS },
            { IdentifiableType.MISC, IdentifiableHandler.MISC_CLASS },
            { IdentifiableType.ANIMAL, IdentifiableHandler.ANIMAL_CLASS },
            { IdentifiableType.WATER, IdentifiableHandler.WATER_CLASS }
        };

        //+ REGISTRATION
        /// <summary>
        /// Registers the prefab of an Identifiable
        /// </summary>
        /// <param name="prefab">The prefab to register</param>
        public static void RegisterPrefab(GameObject prefab)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (prefab.HasComponent(out Identifiable ident))
            {
                if (IDENTIFIABLES.ContainsKey(ident.id)) IDENTIFIABLES[ident.id] = prefab;
                else IDENTIFIABLES.Add(ident.id, prefab);
            }
            
            GuuCore.LOGGER.LogError("Trying to register a identifiable prefab that contains no Identifiable component");
        }

        /// <summary>
        /// Registers a Vac Entry for an Identifiable
        /// </summary>
        /// <param name="vacEntry">The vac entry to register</param>
        public static void RegisterVacEntry(VacItemDefinition vacEntry)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (VAC_ITEM_DEFS.ContainsKey(vacEntry.Id)) VAC_ITEM_DEFS[vacEntry.Id] = vacEntry;
            else VAC_ITEM_DEFS.Add(vacEntry.Id, vacEntry);
        }

        /// <summary>
        /// Registers a new identifiable handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(IdentifiableHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }

        /// <summary>
        /// Registers a new identifiable type
        /// </summary>
        /// <param name="type">The type to register</param>
        /// <param name="hashSet">The hash set used for the listings</param>
        public static void RegisterType(IdentifiableType type, HashSet<Identifiable.Id> hashSet)
        {
            if (ModLoader.CurrentStep != LoadingState.REGISTER)
                throw new Exception("You can only register things during the 'Register' step");
            
            if (!CLASS_LISTING.ContainsKey(type))
            {
                GuuCore.LOGGER.LogError($"The identifiable type {type} is already registered");
                return;
            }
            
            CLASS_LISTING.Add(type, hashSet);
        }
        
        //+ CLASSIFICATION
        /// <summary>
        /// Classifies the identifiable into it's respective class
        /// </summary>
        /// <param name="id">The identifiable to classify</param>
        /// <param name="types">The types to classify as</param>
        public static void Classify(Identifiable.Id id, IEnumerable<IdentifiableType> types)
        {
            foreach (IdentifiableType type in types)
            {
                foreach (IdentifiableHandler handler in HANDLERS)
                    handler.Organize(id, type);
            }
        }
        
        /// <summary>
        /// Checks if an identifiable is from a type
        /// </summary>
        /// <param name="id">The id of the identifiable to check</param>
        /// <param name="type">The type of identifiable to check for</param>
        /// <returns>True if it is a valid type, false otherwise</returns>
        public static bool IsTypeValid(Identifiable.Id id, IdentifiableType type) => CLASS_LISTING.ContainsKey(type) && CLASS_LISTING[type].Contains(id);

        /// <summary>
        /// Populates an hash set with all identifiables inside the class for each type given
        /// </summary>
        /// <param name="toPopulate">The hash set to populate</param>
        /// <param name="types">The types to populate with</param>
        public static void PopulateHashSet(HashSet<Identifiable.Id> toPopulate, params IdentifiableType[] types)
        {
            foreach (IdentifiableType type in types)
                toPopulate.UnionWith(CLASS_LISTING[type]);
        }
    }
}