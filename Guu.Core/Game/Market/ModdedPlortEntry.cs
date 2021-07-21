using System;

namespace Guu.Game
{
    /// <summary>
    /// A modded version of the Market's Plort Entry
    /// </summary>
    [Serializable]
    public class ModdedPlortEntry : MarketUI.PlortEntry
    {
        public int[] unlockCount;

        public bool unlockOnlyWhenCollected;

        public ProgressDirector.ProgressType[] toShow;
        public int[] showCount;

        public bool showOnlyWhenCollected;
    }
}