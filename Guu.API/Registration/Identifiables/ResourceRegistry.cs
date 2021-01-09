using System.Collections.Generic;

namespace Guu.API
{
    /// <summary>
    /// The registry to register all resource related things
    /// </summary>
    public static class ResourceRegistry
    {
        //+ VARIABLES
        private static readonly HashSet<Identifiable.Id> REFINERY_RESOURCES = new HashSet<Identifiable.Id>(Identifiable.idComparer);
        
        private static readonly HashSet<MarketUI.PlortEntry> MARKET_PLORTS = new HashSet<MarketUI.PlortEntry>();
        private static readonly HashSet<EconomyDirector.ValueMap> MARKET_VALUES = new HashSet<EconomyDirector.ValueMap>();
    }
}