using System;
using System.Collections.Generic;
using Guu.Game.General;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all chroma pack related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class ChromaPackRegistry
    {
        //+ VARIABLES
        //? Chroma Pack Related
        private static readonly HashSet<RanchDirector.PaletteEntry> PALETTE_ENTRIES = new HashSet<RanchDirector.PaletteEntry>();
        private static readonly HashSet<RanchDirector.PaletteType> PALETTE_TYPES = new HashSet<RanchDirector.PaletteType>();
        
        //? Registry Related
        private static readonly HashSet<ChromaPackHandler> HANDLERS = new HashSet<ChromaPackHandler> { new ChromaPackHandler().Setup() };
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new chroma pack handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(ChromaPackHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }
    }
}