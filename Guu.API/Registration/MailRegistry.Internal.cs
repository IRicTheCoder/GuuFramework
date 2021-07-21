using System.Linq;
using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all mail related things</summary>
    public static partial class MailRegistry
    {
        //+ HANDLING

        //+ SAVE HANDLING
		internal static bool IsMailRegistered(string mailID) => MAILS.ContainsKey(mailID);
		internal static bool IsTypeRegistered(MailDirector.Type type) => MAIL_TYPES.ContainsKey(type);
	}
}