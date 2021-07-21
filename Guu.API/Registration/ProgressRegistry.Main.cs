using System;
using System.Collections.Generic;
using Guu.Game.General;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all progress related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class ProgressRegistry
    {
        //+ CONSTANTS

        //+ DELEGATES
        
        //+ VARIABLES
        //? Progress Related
        private static readonly HashSet<ProgressDirector.ProgressType> PROGRESS_TYPES = new HashSet<ProgressDirector.ProgressType>(ProgressDirector.progressTypeComparer);
        private static readonly Dictionary<ProgressDirector.ProgressTrackerId, ProgressDirector.DelayedProgressTracker> PROGRESS_TRACKER = new Dictionary<ProgressDirector.ProgressTrackerId, ProgressDirector.DelayedProgressTracker>(ProgressDirector.progressTrackerIdComparer);
        
        //? Registry Related
        private static readonly HashSet<ProgressHandler> HANDLERS = new HashSet<ProgressHandler> { new ProgressHandler().Setup() };
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new progress handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(ProgressHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }
        
        //+ LOCKERS
        
    }
}