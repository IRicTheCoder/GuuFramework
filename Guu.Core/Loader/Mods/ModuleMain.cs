﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Guu.Loader;

namespace Guu
{
    /// <summary>Represents a Module main class</summary>
    public abstract class ModuleMain : IModLoad
    {
        //+ PROPERTIES
        /// <summary>The main assembly for this mod</summary>
        public Assembly Assembly { get; internal set; }

        /// <summary>The Mod main class for the mod this module belongs to</summary>
        public ModMain Main { get; internal set; }

        //+ LOADING PROCESS METHODS
        /// <summary>
        /// The initialization method for the mod. Use this to call and initialize things that
        /// are important for your mod.
        /// </summary>
        protected virtual void Init() { }
        
        /// <summary>
        /// The pre-load method for the mod. Use this to prepare for registering things, like generating
        /// new Largo IDs and other things registration might need.
        /// </summary>
        protected virtual void PreLoad() { }
        
        /// <summary>
        /// The registration method for the mod. Use this to register all things related to the mod, make sure
        /// you initialize everything you need on the <see cref="PreLoad"/> so that, when registering it can be
        /// done in any order. It is also a good place to call any custom registry you might need.
        /// </summary>
        protected virtual void Register() { }
        
        /// <summary>
        /// The load method for the mod. Use this to load anything that requires the GameContext, it is also a
        /// great place to register callbacks and/or events.
        /// </summary>
        protected virtual void Load() { }
        
        /// <summary>
        /// The handle method for the mod. This is when the handlers are called, you can use this to execute
        /// your own custom handlers, that are not part of the registries.
        /// </summary>
        protected virtual void Handle() { }
        
        /// <summary>
        /// The post-load method for the mod. This means everything is loaded and ready to be used, here you
        /// can manipulated prefabs and other things that might require a deeper control, like adding components,
        /// or changing certain values in things.
        /// </summary>
        protected virtual void PostLoad() { }
        
        /// <summary>
        /// The comms method for the mod. Use this to make connections between mods, and communicate between each other.
        /// You can register inter mod messages and register readers. After all Comms methods are executed each message
        /// will be process.
        /// </summary>
        protected virtual void Comms() { }
        
        // Runs each step based on the current load step
        void IModLoad.RunLoadStep()
        {
            switch (Loader.ModLoader.CurrentStep)
            {
                case LoadingState.INIT:
                    Init();
                    break;
                case LoadingState.PRE_LOAD:
                    PreLoad();
                    break;
                case LoadingState.REGISTER:
                    Register();
                    break;
                case LoadingState.LOAD:
                    Load();
                    break;
                case LoadingState.HANDLE:
                    Handle();
                    break;
                case LoadingState.POST_LOAD:
                    PostLoad();
                    break;
                case LoadingState.COMMS:
                    Comms();
                    break;
                case LoadingState.NONE:
                case LoadingState.DONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}