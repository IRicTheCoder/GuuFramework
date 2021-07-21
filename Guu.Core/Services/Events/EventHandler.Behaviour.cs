using System;
using System.Collections;
using UnityEngine;

namespace Guu.Services.Events
{
    /// <summary>The service that allows handling of events</summary>
    public partial class EventHandler
    {
        //+ CONSTANTS
        private static readonly WaitUntil WAIT_UNTIL_CONTEXT = new WaitUntil(() => SR.SceneContextLoaded);
        private static readonly WaitUntil WAIT_UNTIL_WORLD = new WaitUntil(() => Levels.IsLevel(Levels.WORLD));
        private static readonly WaitUntil WAIT_UNTIL_PAUSE = new WaitUntil(() => SR.TimeDir.HasPauser());
        private static readonly WaitForSecondsRealtime SECONDS_REALTIME = new WaitForSecondsRealtime(5f);

        //+ VARIABLES
        private static bool gameContextLoaded;
        private static bool sceneContextLoaded;
        
        //+ BEHAVIOUR
        private void Start()
        {
            StartCoroutine(TimedUpdate());
            
            AmbianceDirector.onAwakeDelegate += dir => OnAmbienceAwake?.Handle(args: new object[]{ dir }, unique: true);
            
            SceneContext.onNextSceneAwake += cxt => OnNextSceneAwake?.Handle(args: new object[]{ cxt }, unique: true);
            SceneContext.onSceneLoaded += cxt => OnSceneLoaded?.Handle(args: new object[]{ cxt }, unique: true);
            SceneContext.beforeSceneLoaded += cxt => BeforeSceneLoaded?.Handle(args: new object[]{ cxt }, unique: true);
        }

        private void Update()
        {
            if (SR.GameContextLoaded && !gameContextLoaded)
            {
                SR.MessageDir.RegisterBundlesListener(dir => OnBundlesAvailable?.Handle(args: new object[]{ dir }, unique: true));
                SR.InputDir.onKeysChanged += () => OnKeysChanged?.Handle(unique: true);
                SR.ProgressDir.onProgressChanged  += () => OnProgressChanged?.Handle(unique: true);
                
                gameContextLoaded = true;
            }
            
            if (SR.SceneContextLoaded && !sceneContextLoaded)
            {
                SR.EcoDir.didUpdateDelegate += () => OnPricesReset?.Handle(unique: true);
                SR.EcoDir.onRegisterSold += id => OnRegisterSold?.Handle(args: new object[]{ id }, unique: true);
                SR.SlimeAppDir.onSlimeAppearanceChanged += (def, app) => OnSlimeAppearanceChanged?.Handle(args: new object[]{ def, app }, unique: true);
                SR.ExchangeDir.onOfferChanged += () => OnOfferChanged?.Handle(unique: true);
                SR.TimeDir.onFastForwardChanged += state => OnFastForward?.Handle(args: new object[]{ state }, unique: true);

                sceneContextLoaded = true;
            }
            
            OnGameUpdate?.Invoke(SR.Game, SR.Scene);
        }

        private void LateUpdate()
        {
            OnGameLateUpdate?.Invoke(SR.Game, SR.Scene);
        }

        private void FixedUpdate()
        {
            OnGameFixedUpdate?.Invoke(SR.Game, SR.Scene);
        }

        //+ COROUTINE
        private IEnumerator TimedUpdate()
        {
            if (!SceneContext.Instance) yield return WAIT_UNTIL_CONTEXT;
            if (!Levels.IsLevel(Levels.WORLD)) yield return WAIT_UNTIL_WORLD;
            
            while (Levels.IsLevel(Levels.WORLD))
            {
                if (SceneContext.Instance.TimeDirector.HasPauser()) yield return WAIT_UNTIL_PAUSE;
                
                OnGameTimedUpdate?.Invoke(SR.Game, SR.Scene);
                yield return SECONDS_REALTIME;
            }
        }
    }
}