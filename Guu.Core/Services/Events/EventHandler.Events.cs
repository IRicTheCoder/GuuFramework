using System;
using System.Diagnostics.CodeAnalysis;
using EdenUnity.Core.Code;
using JetBrains.Annotations;
using UnityEngine;

#pragma warning disable 67
namespace Guu.Services.Events
{
    /// <summary>The service that allows handling of events</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers), SuppressMessage("ReSharper", "DelegateSubtraction")]
    public partial class EventHandler : USingleton<EventHandler>, IServiceInternal
    {
        //+ EVENTS
        //? General Behaviour
        /// <summary>Triggers when the game runs an update cycle</summary>
        public static event Action<GameContext, SceneContext> OnGameUpdate;
        
        /// <summary>Triggers when the game runs a late update cycle</summary>
        public static event Action<GameContext, SceneContext> OnGameLateUpdate;
        
        /// <summary>Triggers when the game runs a fixed update cycle</summary>
        public static event Action<GameContext, SceneContext> OnGameFixedUpdate;
        
        /// <summary>Triggers when the game runs a timed update cycle (every 5 seconds)</summary>
        public static event Action<GameContext, SceneContext> OnGameTimedUpdate;
        
        //? Options
        /// <summary>Triggers when the game's audio levels are changed</summary>
        [Obsolete("NOT IMPLEMENTED YET")]
        public static event Action OnAudioLevelsChanged;

        /// <summary>Triggers when the game's resolution is changed</summary>
        public static event Action OnApplyResolution;
        
        //? Contexts
        /// <summary>Triggers when the Scene Context starts</summary>
        public static event Action<SceneContext> BeforeSceneLoaded;
        
        /// <summary>Triggers when the Scene Context ends the start cycle</summary>
        public static event Action<SceneContext> OnSceneLoaded;

        /// <summary>Triggers when the Scene Context awakes</summary>
        public static event Action<SceneContext> OnNextSceneAwake;
        
        //? Directors
        /// <summary>Triggers when language bundles become available</summary>
        public static event Action<MessageDirector> OnBundlesAvailable;

        /// <summary>Triggers when the ambience director awakes</summary>
        public static event Action<AmbianceDirector> OnAmbienceAwake;

        /// <summary>Triggers when an actor enters a cell</summary>
        [Obsolete("NOT IMPLEMENTED YET")]
        public static event Action<CellDirector, Identifiable.Id> OnAddedToCell;
        
        /// <summary>Triggers when an actor leaves a cell</summary>
        [Obsolete("NOT IMPLEMENTED YET")]
        public static event Action<CellDirector, Identifiable.Id> OnRemovedFromCell;
        
        /// <summary>Triggers when the applied chroma pack changes</summary>
        [Obsolete("NOT IMPLEMENTED YET")]
        public static event Action<RanchDirector.PaletteType, RanchDirector.Palette> OnChromaPackChanged;

        /// <summary>Triggers when the market prices are reset</summary>
        public static event Action OnPricesReset;

        /// <summary>Triggers when a plort is deposited into the market</summary>
        public static event Action<Identifiable.Id> OnRegisterSold;

        /// <summary>Triggers when the exchange offer changes</summary>
        public static event Action OnOfferChanged;

        /// <summary>Triggers when the player changes the keys in options</summary>
        public static event Action OnKeysChanged;
        
        /// <summary>Triggers when the progress changes</summary>
        public static event Action OnProgressChanged;
        
        /// <summary>Triggers when the game gets unpaused</summary>
        [Obsolete("NOT IMPLEMENTED YET")]
        public static event Action OnUnpause;
        
        /// <summary>Triggers when fast forwarding gets activated or deactivate</summary>
        public static event Action<bool> OnFastForward;
        
        //? Slimes
        /// <summary>Triggers when the appearance of a slime is changed</summary>
        public static event Action<SlimeDefinition, SlimeAppearance> OnSlimeAppearanceChanged;

        //? Player State
        /// <summary>Triggers when the player enters a zone</summary>
        public static event Action<PlayerState, ZoneDirector.Zone> OnEnteredZone;

        /// <summary>Triggers when the player unlocks a new zone</summary>
        public static event Action<PlayerState, ZoneDirector.Zone> OnUnlockZone;
        
        /// <summary>Triggers when the player's ammo mode changes</summary>
        public static event Action<PlayerState, PlayerState.AmmoMode> OnSetAmmoMode;

        /// <summary>Triggers when the player's health changes</summary>
        public static event Action<PlayerState, int> OnSetHealth;
        
        /// <summary>Triggers when the player's energy changes</summary>
        public static event Action<PlayerState, int> OnSetEnergy;
        
        /// <summary>Triggers when the player's radiation changes</summary>
        public static event Action<PlayerState, int> OnSetRad;
        
        /// <summary>Triggers when the player's radiation increases</summary>
        public static event Func<PlayerState, float, bool> OnAddRads;

        /// <summary>Triggers when the player's radiation decreases</summary>
        public static event Func<PlayerState, float, bool> OnRemoveRads;

        /// <summary>Triggers when the player gets damaged</summary>
        public static event Func<PlayerState, int, GameObject, bool> OnDamage;
        
        /// <summary>Triggers when the player heals</summary>
        public static event Action<PlayerState, int> OnHeal;

        /// <summary>Triggers when the player spends energy</summary>
        public static event Func<PlayerState, float, bool> OnSpendEnergy;

        /// <summary>Triggers when the player receives currency</summary>
        public static event Action<PlayerState, int, PlayerState.CoinsType> OnAddCurrency;

        /// <summary>Triggers when the player spends currency</summary>
        public static event Func<PlayerState, int, bool, bool> OnSpendCurrency;

        /// <summary>Triggers when the player receives a slime key</summary>
        public static event Action<PlayerState> OnAddKey;

        /// <summary>Triggers when the player spends a slime key</summary>
        public static event Action<PlayerState> OnSpendKey;

        /// <summary>Triggers when the player receives an upgrade</summary>
        public static event Action<PlayerState, PlayerState.Upgrade, bool> OnAddUpgrade;

        /// <summary>Triggers when the player exists a zone</summary>
        public static event Action<PlayerState, ZoneDirector.Zone> OnExitedZone;

        /// <summary>Triggers when the player end the game's story</summary>
        public static event Action<PlayerState> OnEndGame;

        /// <summary>Triggers when the gadget mode changes</summary>
        public static event Func<PlayerState, bool, bool> OnGadgetModeChanged;
    }
}