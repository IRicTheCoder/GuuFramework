using System.Linq;
using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all exchange related things</summary>
    public static partial class ExchangeRegistry
    {
        //+ HANDLING

        //+ SAVE HANDLING
        internal static bool IsCategoryRegistered(ExchangeDirector.Category category) => CATEGORIES.ContainsKey(category);
        internal static bool IsOfferTypeRegistered(ExchangeDirector.OfferType type) => OFFER_TYPES.Contains(type);
		internal static bool IsNonIdenRegistered(ExchangeDirector.NonIdentReward reward) => NON_IDENT_REWARDS.Count(r => r.reward == reward) > 0;
	}
}