using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Eden.Patching.Harmony;
using Guu.API;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Guu.Patches.Registration
{
    [EdenHarmony.Wrapper(typeof(LookupDirector))]
    [UsedImplicitly]
    internal static class LookupDirector_Patch
    {
        [UsedImplicitly]
        private static void Awake_Prefix(LookupDirector @this)
        {
        }
        
        [UsedImplicitly]
        private static void Awake_Postfix(LookupDirector @this, Dictionary<Identifiable.Id, GameObject> p_identifiablePrefabDict, 
                                          Dictionary<Gadget.Id, GadgetDefinition> p_gadgetDefinitionDict, Dictionary<Identifiable.Id, VacItemDefinition> p_vacItemDict)
        {
            //& Injects main game content
            IdentifiableRegistry.InjectPrefabs(p_identifiablePrefabDict);
            GadgetRegistry.InjectGadgets(p_gadgetDefinitionDict);
            IdentifiableRegistry.InjectVacEntries(p_vacItemDict);
        }
    }
}