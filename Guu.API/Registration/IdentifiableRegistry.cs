using System.Collections.Generic;
using UnityEngine;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all identifiable related things
    /// </summary>
    public static class IdentifiableRegistry
    {
        //+ VARIABLES
        private static readonly Dictionary<Identifiable.Id, VacItemDefinition> VAC_ITEM_DEFS = new Dictionary<Identifiable.Id, VacItemDefinition>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, GameObject> IDENTIFIABLES = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);
        
        private static readonly List<IdentifiableHandler> HANDLERS = new List<IdentifiableHandler> { new IdentifiableHandler().Setup() };
        
        private static readonly Dictionary<IdentifiableType, HashSet<Identifiable.Id>> CLASS_LISTING = new Dictionary<IdentifiableType, HashSet<Identifiable.Id>>
        {
            { IdentifiableType.VEGGIE, Identifiable.VEGGIE_CLASS },
            { IdentifiableType.FRUIT, Identifiable.FRUIT_CLASS },
            { IdentifiableType.MEAT, Identifiable.MEAT_CLASS },
            { IdentifiableType.TOFU, Identifiable.TOFU_CLASS },
            { IdentifiableType.SLIME, Identifiable.SLIME_CLASS },
            { IdentifiableType.LARGO, Identifiable.LARGO_CLASS },
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
            if (prefab.HasComponent(out Identifiable ident))
            {
                if (IDENTIFIABLES.ContainsKey(ident.id)) IDENTIFIABLES[ident.id] = prefab;
                else IDENTIFIABLES.Add(ident.id, prefab);
            }
            
            Debug.LogError($"Trying to register a identifiable prefab that contains no Identifiable component");
        }

        /// <summary>
        /// Registers a Vac Entry for an Identifiable
        /// </summary>
        /// <param name="vacEntry">The vac entry to register</param>
        public static void RegisterVacEntry(VacItemDefinition vacEntry)
        {
            if (VAC_ITEM_DEFS.ContainsKey(vacEntry.Id)) VAC_ITEM_DEFS[vacEntry.Id] = vacEntry;
            else VAC_ITEM_DEFS.Add(vacEntry.Id, vacEntry);
        }

        /// <summary>
        /// Registers a fashion pod for a fashion
        /// </summary>
        /// <param name="pod">The pod to register</param>
        public static void RegisterPodForFashion(GadgetDefinition pod)
        {
            Dictionary<Identifiable.Id, Gadget.Id> gadgetNameDict = typeof(Identifiable).GetPrivateField<Dictionary<Identifiable.Id, Gadget.Id>>("GADGET_NAME_DICT");
            
            gadgetNameDict.Add(pod.prefab.GetComponent<FashionPod>().fashionId, pod.id);
        }
        
        //+ CLASSIFICATION
        /// <summary>
        /// Adds an identifiable to a type
        /// </summary>
        /// <param name="id">The identifiable to be added</param>
        /// <param name="type">The type to add to</param>
        /// <returns>True if the identifiable got added, false otherwise</returns>
        public static bool AddToType(Identifiable.Id id, IdentifiableType type)
        {
            if (!CLASS_LISTING.ContainsKey(type))
            {
                Debug.LogError($"Trying to add identifiable {id} to type {type}. but type listing isn't registered");
                return false;
            }

            CLASS_LISTING[type].Add(id);

            return true;
        }

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
        public static bool IsTypeValid(Identifiable.Id id, IdentifiableType type)
        {
            return CLASS_LISTING.ContainsKey(type) && CLASS_LISTING[type].Contains(id);
        }
        
        //+ HANDLING
        internal static void InjectPrefabs(Dictionary<Identifiable.Id, GameObject> prefabs)
        {
            foreach (Identifiable.Id id in IDENTIFIABLES.Keys)
            {
                if (prefabs.ContainsKey(id))
                {
                    Debug.LogError($"Couldn't inject prefab for identifiable {id}, it is already present in the game");
                    continue;
                }
                
                prefabs.Add(id, IDENTIFIABLES[id]);
            }
        }

        internal static void InjectVacEntries(Dictionary<Identifiable.Id, VacItemDefinition> defs)
        {
            foreach (Identifiable.Id id in VAC_ITEM_DEFS.Keys)
            {
                if (defs.ContainsKey(id))
                {
                    Debug.LogError($"Couldn't inject vac entry for identifiable {id}, it is already present in the game");
                    continue;
                }
                
                defs.Add(id, VAC_ITEM_DEFS[id]);
            }
        }

        internal static void RunHandlers()
        {
            foreach (IdentifiableHandler handler in HANDLERS)
            {
                handler.Handle();
                handler.ClearMemory();
            }
            
            HANDLERS.Clear();
        }
    }
}