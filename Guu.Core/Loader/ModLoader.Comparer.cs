using System.Collections.Generic;
using Guu.Utils;

namespace Guu.Loader
{
    /// <summary>
    /// The Mod Loader, this is the class that will load all mods that are using Guu's modding identification
    /// system. Access this to check if a mod is loaded and get info on any mod.
    /// </summary>
    public static partial class ModLoader
    {
        //+ COMPARER CLASS
        internal class ModComparer : IComparer<GuuMod>
        {
            public int Compare(GuuMod x, GuuMod y)
            {
                if (x == null || y == null) return 0;
                
                string modid0 = x.ID;
                string modid1 = y.ID;
                
                HashSet<string> loadBefore0 = new HashSet<string>(x.LoadBefore);
                HashSet<string> loadAfter0 = new HashSet<string>(x.LoadAfter);
                
                HashSet<string> loadBefore1 = new HashSet<string>(y.LoadBefore);
                HashSet<string> loadAfter1 = new HashSet<string>(y.LoadAfter);

                if (loadBefore0.Contains(modid1)) return -1;
                if (loadBefore1.Contains(modid0)) return 1;

                if (loadAfter0.Contains(modid1)) return 1;
                if (loadAfter1.Contains(modid0)) return -1;
                
                return 0;
            }
        }
    }
}