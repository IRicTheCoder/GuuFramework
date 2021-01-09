using System.Collections.Generic;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all slime related things
    /// </summary>
    public static class SlimeRegistry
    {
        //+ VARIABLES
        private static readonly Dictionary<Identifiable.Id, SlimeDefinition> SLIME_DEFS = new Dictionary<Identifiable.Id, SlimeDefinition>(Identifiable.idComparer);
    }
}