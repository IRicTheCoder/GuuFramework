using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all Gadget related things</summary>
    public static partial class GadgetRegistry
    {
        //+ CLASSIFICATION
        internal static bool AddToType(GadgetDefinition gadget, GadgetType type)
        {
            if (!CLASS_LISTING.ContainsKey(type))
            {
                GuuCore.LOGGER.LogError($"Trying to add gadget {gadget.id} to type {type}, but type listing isn't registered");
                return false;
            }

            CLASS_LISTING[type].Add(gadget.id);
            return true;
        }
        
        //+ HANDLING
        internal static void InjectBlueprints(GadgetsModel model)
        {
            if (!SceneContext.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled) return;

            model.availBlueprints.AddAll(BLUEPRINTS.Keys, id => !BLUEPRINTS[id]);
            model.blueprints.AddAll(BLUEPRINTS.Keys, id => BLUEPRINTS[id]);
        }

        internal static void InjectLocks(GadgetDirector dir)
        {
            if (!SceneContext.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled) return;

            foreach (Gadget.Id id in BLUEPRINT_LOCKS.Keys)
                dir.blueprintLocks.Add(id, new GadgetDirector.BlueprintLocker(dir, id, BLUEPRINT_LOCKS[id].Item1, BLUEPRINT_LOCKS[id].Item2));
        }

        internal static void InjectGadgets(Dictionary<Gadget.Id, GadgetDefinition> defs)
        {
            foreach (Gadget.Id id in GADGETS.Keys)
            {
                if (defs.ContainsKey(id))
                {
                    Debug.LogError($"Couldn't inject gadget {id}, it is already present in the game");
                    continue;
                }
                
                defs.Add(id, GADGETS[id]);
            }
        }

        internal static int GetBuyCount(Gadget.Id id) => GADGET_BUY_COUNT.ContainsKey(id) ? GADGET_BUY_COUNT[id] : 1;
        
        //+ SAVE HANDLING
        internal static bool IsGadgetRegistered(Gadget.Id id) => GADGETS.ContainsKey(id);
    }
}