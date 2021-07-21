using System.Linq;
using MonomiPark.SlimeRancher.DataModel;

namespace Guu.API
{
    /// <summary>The registry to register all chroma pack related things</summary>
    public static partial class ChromaPackRegistry
    {
        //+ HANDLING

        //+ SAVE HANDLING
		internal static bool IsTypeRegistered(RanchDirector.PaletteType type) => PALETTE_TYPES.Contains(type);
		internal static bool IsPaletteRegistered(RanchDirector.Palette palette) => PALETTE_ENTRIES.Count(p => p.palette == palette) > 0;
	}
}