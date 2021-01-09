using System.Collections.Generic;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all toy related things
    /// </summary>
    public static class ToyRegistry
    {
        //+ VARIABLES
        // The bool in the values represents if the toy is an upgraded toy or not
        private static readonly Dictionary<Identifiable.Id, bool> COMMON_TOYS = new Dictionary<Identifiable.Id, bool>(Identifiable.idComparer);
        private static readonly Dictionary<Identifiable.Id, ToyDefinition> TOY_DEFS = new Dictionary<Identifiable.Id, ToyDefinition>(Identifiable.idComparer);
    }
}