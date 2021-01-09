using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all Gadget related things
    /// </summary>
    public static class GadgetRegistry
    {
        //+ DELEGATES
        public delegate GadgetDirector.BlueprintLocker CreateLocker(GadgetDirector dir);
        
        //+ VARIABLES
        private static readonly Dictionary<Gadget.Id, CreateLocker> BLUEPRINT_LOCKS = new Dictionary<Gadget.Id, CreateLocker>(Gadget.idComparer);
        
        // The bool in the values represents if it is unlocked from the start or not
        private static readonly Dictionary<Gadget.Id, bool> BLUEPRINTS = new Dictionary<Gadget.Id, bool>(Gadget.idComparer);
        private static readonly Dictionary<Gadget.Id, GadgetDefinition> GADGETS = new Dictionary<Gadget.Id, GadgetDefinition>(Gadget.idComparer);
        
        private static readonly List<GadgetHandler> HANDLERS = new List<GadgetHandler> { new GadgetHandler().Setup() };

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
        public static void RegisterGadget(GadgetDefinition gadget, bool startAvailable, bool startUnlocked = false)
        {
            if (GADGETS.ContainsKey(gadget.id)) GADGETS[gadget.id] = gadget;
            else GADGETS.Add(gadget.id, gadget);
            
            if (startAvailable || startUnlocked)
                BLUEPRINTS.Add(gadget.id, startUnlocked);
        }

        /// <summary>
        /// Registers the lock for a blueprint
        /// </summary>
        /// <param name="gadget">The gadget to register the lock for</param>
        /// <param name="lockCheck">The lock method</param>
        public static void RegisterLock(GadgetDefinition gadget, CreateLocker lockCheck)
        {
            if (BLUEPRINTS.ContainsKey(gadget.id)) BLUEPRINTS.Remove(gadget.id);

            if (BLUEPRINT_LOCKS.ContainsKey(gadget.id)) BLUEPRINT_LOCKS[gadget.id] = lockCheck;
            else BLUEPRINT_LOCKS.Add(gadget.id, lockCheck);
        }

        /// <summary>
        /// Registers a new gadget handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(GadgetHandler handler)
        {
            if (HANDLERS.Exists(h => h.Owner.Equals(handler.Owner))) return;
            
            HANDLERS.Add(handler.Setup());
        }

        /// <summary>
        /// Registers a new gadget type
        /// </summary>
        /// <param name="type">The type to register</param>
        /// <param name="hashSet">The hash set used for the listings</param>
        public static void RegisterType(GadgetType type, HashSet<Gadget.Id> hashSet)
        {
            if (!CLASS_LISTING.ContainsKey(type))
            {
                Debug.LogError($"The gadget type {type} is already registered");
                return;
            }
            
            CLASS_LISTING.Add(type, hashSet);
        }
        
        //+ CLASSIFICATION
        /// <summary>
        /// Adds a gadget to a type
        /// </summary>
        /// <param name="gadget">The gadget to be added</param>
        /// <param name="type">The type to add to</param>
        /// <returns>True is gadget got added, false otherwise</returns>
        public static bool AddToType(GadgetDefinition gadget, GadgetType type)
        {
            if (!CLASS_LISTING.ContainsKey(type))
            {
                Debug.LogError($"Trying to add gadget {gadget.id} to type {type}, but type listing isn't registered");
                return false;
            }

            CLASS_LISTING[type].Add(gadget.id);

            return true;
        }
        
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
        
        //+ HANDLING
        internal static void InjectBlueprints(GadgetsModel model)
        {
            foreach (Gadget.Id id in BLUEPRINTS.Keys)
            {
                model.availBlueprints.Add(id);
                if (BLUEPRINTS[id]) model.blueprints.Add(id);
            }
        }

        internal static void InjectLocks(GadgetDirector dir)
        {
            if (!SceneContext.Instance.GameModeConfig.GetModeSettings().blueprintsEnabled) return;

            foreach (Gadget.Id id in BLUEPRINT_LOCKS.Keys)
            {
                if (dir.blueprintLocks.ContainsKey(id)) 
                    dir.blueprintLocks[id] = BLUEPRINT_LOCKS[id].Invoke(dir);
                else 
                    dir.blueprintLocks.Add(id, BLUEPRINT_LOCKS[id].Invoke(dir));
            }
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
        
        internal static void RunHandlers()
        {
            foreach (GadgetHandler handler in HANDLERS)
            {
                handler.Handle();
                handler.ClearMemory();
            }
            
            HANDLERS.Clear();
        }
    }
}