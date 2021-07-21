using System;
using System.Collections.Generic;
using Guu.Game.General;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all mail related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class MailRegistry
    {
        //+ CONSTANTS

        //+ DELEGATES
        
        //+ VARIABLES
        //? Mail Related
        private static readonly Dictionary<string, MailDirector.Mail> MAILS = new Dictionary<string, MailDirector.Mail>();
        private static readonly Dictionary<MailDirector.Type, Tuple<Sprite, Sprite>> MAIL_TYPES = new Dictionary<MailDirector.Type, Tuple<Sprite, Sprite>>();
        
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