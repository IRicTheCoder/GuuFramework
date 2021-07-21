using System.Linq;
using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all progress related things</summary>
    public static partial class ProgressRegistry
    {
        //+ HANDLING

        //+ SAVE HANDLING
		internal static bool IsTypeRegistered(ProgressDirector.ProgressType type) => PROGRESS_TYPES.Contains(type);
		internal static bool IsTrackerRegistered(ProgressDirector.ProgressTrackerId tracker) => PROGRESS_TRACKER.ContainsKey(tracker);
	}
}