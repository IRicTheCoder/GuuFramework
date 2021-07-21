using System.Collections.Generic;
using Guu.Loader;

namespace Guu.API
{
    /// <summary>
    /// Serves as a handler for gadgets after being registered, mostly to sort them
    /// and identify them. Make a child of this class to create your own handlers
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class GadgetHandler : IHandler<GadgetHandler>
    {
        //+ CLASS LISTING
        internal static readonly HashSet<Gadget.Id> RANCH_TECH_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);
        internal static readonly HashSet<Gadget.Id> PORTABLE_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);
        
        //+ HANDLING
        internal GadgetHandler InternalSetup()
        {
            RANCH_TECH_CLASS.Add(Gadget.Id.MARKET_LINK);
            RANCH_TECH_CLASS.Add(Gadget.Id.REFINERY_LINK);

            PORTABLE_CLASS.Add(Gadget.Id.PORTABLE_WATER_TAP);
            PORTABLE_CLASS.Add(Gadget.Id.PORTABLE_SLIME_BAIT_FRUIT);
            PORTABLE_CLASS.Add(Gadget.Id.PORTABLE_SCARECROW);
            PORTABLE_CLASS.Add(Gadget.Id.PORTABLE_SLIME_BAIT_VEGGIE);
            PORTABLE_CLASS.Add(Gadget.Id.PORTABLE_SLIME_BAIT_MEAT);

            Setup();
            return this;
        }
        
        /// <inheritdoc />
        public virtual GadgetHandler Setup()
        {
            APIHandler.HandleRegistration += RegistryHandle;
            APIHandler.HandleItems += Handle;
            APIHandler.ClearMemory += ClearMemory;
            
            return this;
        }
        
        /// <summary>
        /// Organizes the gadget by adding it to its list
        /// </summary>
        /// <param name="gadget">The gadget to organize</param>
        /// <param name="type">The type of gadget</param>
        public virtual void Organize(GadgetDefinition gadget, GadgetType type)
        {
            if (!GadgetRegistry.AddToType(gadget, type)) return;

            if (type == GadgetType.FASHION_POD)
            {
                Gadget.RegisterFashion(gadget.id);
                IdentifiableRegistry.RegisterPodForFashion(gadget);
            }
        }

        /// <inheritdoc />
        public virtual void RegistryHandle() { }
        
        /// <inheritdoc />
        public virtual void Handle() { }

        /// <inheritdoc />
        public virtual void ClearMemory() { }

        //+ VERIFICATION
        ///<summary>Checks if the ID is an extractor</summary>
        public static bool IsExtractor(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.EXTRACTOR);

        ///<summary>Checks if the ID is a teleporter</summary>
        public static bool IsTeleporter(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.TELEPORTER);

        ///<summary>Checks if the ID is a warp depot</summary>
        public static bool IsWarpDepot(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.WARP_DEPOT);

        ///<summary>Checks if the ID is misc</summary>
        public static bool IsMisc(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.MISC);

        ///<summary>Checks if the ID is an echo net</summary>
        public static bool IsEchoNet(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.ECHO_NET);

        ///<summary>Checks if the ID is a drone</summary>
        public static bool IsDrone(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.DRONE);

        ///<summary>Checks if the ID is a lamp</summary>
        public static bool IsLamp(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.LAMP);

        ///<summary>Checks if the ID is a fashion pod</summary>
        public static bool IsFashionPod(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.FASHION_POD);

        ///<summary>Checks if the ID is a snare</summary>
        public static bool IsSnare(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.SNARE);

        ///<summary>Checks if the ID is a decoration</summary>
        public static bool IsDeco(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.DECO);

        ///<summary>Checks if the ID is a portable device</summary>
        public static bool IsPortable(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.PORTABLE);

        ///<summary>Checks if the ID is a ranch tech</summary>
        public static bool IsRanchTech(Gadget.Id id) => GadgetRegistry.IsTypeValid(id, GadgetType.RANCH_TECH);
    }
}