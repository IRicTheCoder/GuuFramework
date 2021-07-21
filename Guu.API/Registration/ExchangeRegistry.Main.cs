using System;
using System.Collections.Generic;
using Guu.Game.General;
using Guu.Loader;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
    /// <summary>The registry to register all exchange related things</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static partial class ExchangeRegistry
    {
        //+ CONSTANTS
        /// <summary>Ranch ID for Thora</summary>
        public const string THORA_ID = "thora";
        
        /// <summary>Ranch ID for Viktor</summary>
        public const string VIKTOR_ID = "viktor";
        
        /// <summary>Ranch ID for Ogden</summary>
        public const string OGDEN_ID = "ogden";
        
        /// <summary>Ranch ID for Mochi</summary>
        public const string MOCHI_ID = "mochi";
        
        /// <summary>Ranch ID for Bob</summary>
        public const string BOB_ID = "bob";
        
        //+ DELEGATES
        /// <summary>Represents a lock to check if a rancher can appear in the exchange</summary>
        public delegate bool CanRancherAppear(string ranchID);
        
        /// <summary>Represents a lock to check if a non identifiable entry can appear in the exchange</summary>
        public delegate bool CanNonIdentAppear(ExchangeDirector.NonIdentReward reward);
        
        //+ VARIABLES
        //? Rancher Related
        private static readonly HashSet<ExchangeDirector.Rancher> RANCHERS = new HashSet<ExchangeDirector.Rancher>();
        private static readonly Dictionary<string, List<RancherLocker>> RANCHER_LOCKS = new Dictionary<string, List<RancherLocker>>();
        
        private static readonly Dictionary<string, HashSet<ExchangeDirector.Category>> REQUEST_CATEGORIES = new Dictionary<string, HashSet<ExchangeDirector.Category>>();
        private static readonly Dictionary<string, HashSet<ExchangeDirector.Category>> REWARD_CATEGORIES = new Dictionary<string, HashSet<ExchangeDirector.Category>>();
        private static readonly Dictionary<string, HashSet<ExchangeDirector.Category>> RARE_REWARD_CATEGORIES = new Dictionary<string, HashSet<ExchangeDirector.Category>>();
        
        private static readonly Dictionary<string, HashSet<Identifiable.Id>> REQUEST_IDENTS = new Dictionary<string, HashSet<Identifiable.Id>>();
        private static readonly Dictionary<string, HashSet<Identifiable.Id>> REWARD_IDENTS = new Dictionary<string, HashSet<Identifiable.Id>>();
        private static readonly Dictionary<string, HashSet<Identifiable.Id>> RARE_REWARD_IDENTS = new Dictionary<string, HashSet<Identifiable.Id>>();
        
        //? Offer Related
        private static readonly HashSet<ExchangeDirector.OfferType> OFFER_TYPES = new HashSet<ExchangeDirector.OfferType>();
        private static readonly Dictionary<ExchangeDirector.Category, HashSet<Identifiable.Id>> CATEGORIES = new Dictionary<ExchangeDirector.Category, HashSet<Identifiable.Id>>();
        
        private static readonly HashSet<Identifiable.Id> UNLOCKED_IDENTS = new HashSet<Identifiable.Id>();
        private static readonly HashSet<ExchangeDirector.UnlockList> UNLOCK_LISTS = new HashSet<ExchangeDirector.UnlockList>();
        
        private static readonly HashSet<ExchangeDirector.NonIdentEntry> NON_IDENT_REWARDS = new HashSet<ExchangeDirector.NonIdentEntry>();
        private static readonly Dictionary<ExchangeDirector.NonIdentReward, List<NonIdentLocker>> NON_IDENT_LOCKS = new Dictionary<ExchangeDirector.NonIdentReward, List<NonIdentLocker>>();
        
        //? Registry Related
        private static readonly HashSet<ExchangeHandler> HANDLERS = new HashSet<ExchangeHandler> { new ExchangeHandler().Setup() };
        
        //+ REGISTRATION
        /// <summary>
        /// Registers a new exchange handler
        /// </summary>
        /// <param name="handler">The handler to register</param>
        public static void RegisterHandler(ExchangeHandler handler)
        {
            if (ModLoader.CurrentStep != LoadingState.PRE_LOAD)
                throw new Exception("Handlers need to be registered during 'Pre-Load'");
            
            HANDLERS.Add(handler.Setup());
        }
        
        //+ LOCKERS
        /// <summary>A locker for the ranchers in exchange</summary>
        [UsedImplicitly]
        public class RancherLocker : LockerBase<CanRancherAppear>
        {
            /// <inheritdoc />
            public RancherLocker(CanRancherAppear check) : base(check) { }

            /// <summary>
            /// Checks if the rancher is unlocked
            /// </summary>
            /// <param name="rancherID">The ID to check</param>
            /// <returns>True if unlocked, false otherwise</returns>
            public bool IsUnlocked(string rancherID)
            {
                if (unlocked) return true;

                unlocked = lockerCheck?.Invoke(rancherID) ?? true;
                return unlocked;
            }
        }
        
        /// <summary>A locker for the non identifiable rewards in the exchange</summary>
        [UsedImplicitly]
        public class NonIdentLocker : LockerBase<CanNonIdentAppear>
        {
            /// <inheritdoc />
            public NonIdentLocker(CanNonIdentAppear check) : base(check) { }

            /// <summary>
            /// Checks if the non ident is unlocked
            /// </summary>
            /// <param name="reward">The ID to check</param>
            /// <returns>True if unlocked, false otherwise</returns>
            public bool IsUnlocked(ExchangeDirector.NonIdentReward reward)
            {
                if (unlocked) return true;

                unlocked = lockerCheck?.Invoke(reward) ?? true;
                return unlocked;
            }
        }
    }
}