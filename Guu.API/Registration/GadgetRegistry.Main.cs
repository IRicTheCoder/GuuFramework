using System;
using System.Collections.Generic;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all Gadget related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class GadgetRegistry
    {
        //+ VARIABLES
        //? Blueprint Related
        private static readonly Dictionary<Gadget.Id, Tuple<GadgetDirector.UnlockCondition, float>> BLUEPRINT_LOCKS = new Dictionary<Gadget.Id, Tuple<GadgetDirector.UnlockCondition, float>>(Gadget.idComparer);
        private static readonly Dictionary<Gadget.Id, bool> BLUEPRINTS = new Dictionary<Gadget.Id, bool>(Gadget.idComparer);
        
        //? Gadget Related
        private static readonly Dictionary<Gadget.Id, GadgetDefinition> GADGETS = new Dictionary<Gadget.Id, GadgetDefinition>(Gadget.idComparer);
        private static readonly Dictionary<Gadget.Id, int> GADGET_BUY_COUNT = new Dictionary<Gadget.Id, int>(Gadget.idComparer);
        
        //? Registry Related
        private static readonly HashSet<GadgetHandler> HANDLERS = new HashSet<GadgetHandler> { new GadgetHandler().InternalSetup() };

        private static readonly Dictionary<GadgetType, HashSet<Gadget.Id>> CLASS_LISTING = new Dictionary<GadgetType, HashSet<Gadget.Id>>
        {
            { GadgetType.MISC, Gadget.MISC_CLASS },
            { GadgetType.EXTRACTOR, Gadget.EXTRACTOR_CLASS },
            { GadgetType.TELEPORTER, Gadget.TELEPORTER_CLASS },
            { GadgetType.WARP_DEPOT, Gadget.WARP_DEPOT_CLASS },
            { GadgetType.ECHO_NET, Gadget.ECHO_NET_CLASS },
            { GadgetType.LAMP, Gadget.LAMP_CLASS },
            { GadgetType.FASHION_POD, Gadget.FASHION_POD_CLASS },
            { GadgetType.SNARE, Gadget.SNARE_CLASS },
            { GadgetType.DECO, Gadget.DECO_CLASS },
            { GadgetType.DRONE, Gadget.DRONE_CLASS },
            { GadgetType.RANCH_TECH, GadgetHandler.RANCH_TECH_CLASS },
            { GadgetType.PORTABLE, GadgetHandler.PORTABLE_CLASS }
        };

        //+ REGISTRATION
        /// <summary>
        /// Registers the Gadget
        /// </summary>
        /// <param name="gadget">The gadget definition</param>
        /// <param name="startAvailable">Should it's blueprint start available?</param>
        /// <param name="startUnlocked">Should it's blueprint start unlocked?</param>
        /// <param name="buyCount">The amount to get when you buy the gadget on the shop. 1 is the default, and 2 just sets the definition to buy in pairs</param>
        public static void RegisterGadget(GadgetDefinition gadget, bool startAvailable, bool startUnlocked = false, int buyCount = 1)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");

            if (buyCount == 2) gadget.buyInPairs = true;
            if (buyCount > 2) GADGET_BUY_COUNT[gadget.id] = buyCount;
            GADGETS[gadget.id] = gadget;

            if ((startAvailable || startUnlocked) && !BLUEPRINT_LOCKS.ContainsKey(gadget.id))
                BLUEPRINTS[gadget.id] = startUnlocked;
        }

        /// <summary>
        /// Registers the lock for a blueprint
        /// </summary>
        /// <param name="gadget">The gadget to register the lock for</param>
        /// <param name="unlockCondition">The unlock condition</param>
        /// <param name="unlockDelayHrs">The amount of game hours to delay</param>
        public static void RegisterLock(GadgetDefinition gadget, GadgetDirector.UnlockCondition unlockCondition, float unlockDelayHrs)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            if (BLUEPRINTS.ContainsKey(gadget.id)) BLUEPRINTS.Remove(gadget.id);
            BLUEPRINT_LOCKS[gadget.id] = new Tuple<GadgetDirector.UnlockCondition, float>(unlockCondition, unlockDelayHrs);
        }

        /// <summary>
        /// Registers a new gadget handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(GadgetHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }

        /// <summary>
        /// Registers a new gadget type
        /// </summary>
        /// <param name="type">The type to register</param>
        /// <param name="hashSet">The hash set used for the listings</param>
        public static void RegisterType(GadgetType type, HashSet<Gadget.Id> hashSet)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            if (!CLASS_LISTING.ContainsKey(type))
            {
                Debug.LogError($"The gadget type {type} is already registered");
                return;
            }
            
            CLASS_LISTING.Add(type, hashSet);
        }
        
        //+ CLASSIFICATION
        /// <summary>
        /// Classifies the gadget into it's respective class
        /// </summary>
        /// <param name="gadget">The gadget to classify</param>
        /// <param name="types">The types to classify as</param>
        public static void Classify(GadgetDefinition gadget, IEnumerable<GadgetType> types)
        {
            foreach (GadgetType type in types)
            {
                foreach (GadgetHandler handler in HANDLERS)
                    handler.Organize(gadget, type);
            }
        }

        /// <summary>
        /// Checks if a gadget is from a type
        /// </summary>
        /// <param name="id">The id of the gadget to check</param>
        /// <param name="type">The type of gadget to check for</param>
        /// <returns>True if it is a valid type, false otherwise</returns>
        public static bool IsTypeValid(Gadget.Id id, GadgetType type)
        {
            return CLASS_LISTING.ContainsKey(type) && CLASS_LISTING[type].Contains(id);
        }
        
        /// <summary>
        /// Populates an hash set with all gadgets inside the class for each type given
        /// </summary>
        /// <param name="toPopulate">The hash set to populate</param>
        /// <param name="types">The types to populate with</param>
        public static void PopulateHashSet(HashSet<Gadget.Id> toPopulate, params GadgetType[] types)
        {
            foreach (GadgetType type in types)
                toPopulate.UnionWith(CLASS_LISTING[type]);
        }
    }
}