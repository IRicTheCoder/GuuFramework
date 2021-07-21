using System;
using System.Collections.Generic;
using Guu.Loader;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all identifiable related things</summary>
    public static partial class IdentifiableRegistry
    {
        //+ INTERNAL REGISTRATION
        internal static void RegisterPodForFashion(GadgetDefinition pod)
        {
            Dictionary<Identifiable.Id, Gadget.Id> gadgetNameDict = typeof(Identifiable).GetPrivateField<Dictionary<Identifiable.Id, Gadget.Id>>("GADGET_NAME_DICT");
            gadgetNameDict.Add(pod.prefab.GetComponent<FashionPod>().fashionId, pod.id);
        }
        
        //+ CLASSIFICATION
        internal static void AddToType(Identifiable.Id id, IdentifiableType type)
        {
            if (!CLASS_LISTING.ContainsKey(type))
            {
                GuuCore.LOGGER.LogError($"Trying to add identifiable {id} to type {type}. but type listing isn't registered");
                return;
            }

            CLASS_LISTING[type].Add(id);
        }

        //+ HANDLING
        internal static void InjectPrefabs(Dictionary<Identifiable.Id, GameObject> prefabs)
        {
            foreach (Identifiable.Id id in IDENTIFIABLES.Keys)
            {
                if (prefabs.ContainsKey(id))
                {
                    GuuCore.LOGGER.LogError($"Couldn't inject prefab for identifiable {id}, it is already present in the game");
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
                    GuuCore.LOGGER.LogError($"Couldn't inject vac entry for identifiable {id}, it is already present in the game");
                    continue;
                }
                
                defs.Add(id, VAC_ITEM_DEFS[id]);
            }
        }
        
        //+ SAVE HANDLING
        internal static bool IsIdentifiableRegistered(Identifiable.Id id) => IDENTIFIABLES.ContainsKey(id);
    }
}