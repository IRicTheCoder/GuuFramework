using System;
using System.Collections.Generic;
using Guu.Game.General;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all id related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class IDRegistry
    {
        //+ CONSTANTS
        internal const string GUU_PREFIX = "<guu>";
        
        //+ VARIABLES
        //? Actor Related
        private static readonly HashSet<long> ACTOR_IDS = new HashSet<long>();
            
        //? Registry Related
        private static readonly HashSet<IDHandler> HANDLERS = new HashSet<IDHandler> { new IDHandler().Setup() };
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new chroma pack handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(IDHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }
    }
}