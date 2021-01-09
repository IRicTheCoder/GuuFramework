using System;
using System.Collections.Generic;
using Guu.SaveGame;
using UnityEngine;

namespace Guu.SaveSystem
{
    ///<summary>Contains all custom data for mods</summary>
    public partial class ModdedData : MonoBehaviour
    {
        //+ EVENTS
        ///<summary>Sets up the modded data system</summary>
        public static event Action Setup;

        ///<summary>Processes the loading for each modded data object</summary>
        public static event Action<ModdedData> ProcessLoad;

        //+ VARIABLES
        // The modded data
        internal readonly Dictionary<string, object> data = new Dictionary<string, object>();
        
        // Conversion Callbacks
        internal static readonly Dictionary<string, SaveGame.ModdedData.ConversionFunction> KEY_CONVERSIONS = 
            new Dictionary<string, SaveGame.ModdedData.ConversionFunction>();

        // Starts the functions
        private void Start()
        {
            if (this.HasComponent(out Identifiable iden)) 
                ModdedSaveHandler.RegisterModdedData(this, "ident:" + iden.GetActorId());
            
            if (GetComponent<Identifiable>().id != Identifiable.Id.TABBY_SLIME) return;
            
            AddKeyConversion("test", SaveGame.ModdedData.STRING);
            if (!data.ContainsKey("test"))
            {
                Debug.Log("Test key not found");
                Set("test", "something");
            }
            else
            {
                Debug.Log($"Test key is: {Get("test", "abc")}");
            }
        }

        //+ DATA CONTROL
        /// <summary>
        /// Sets a data value for this object (or adds it if not found)
        /// </summary>
        /// <param name="key">The key to identify the value</param>
        /// <param name="value">The value to add</param>
        /// <typeparam name="T">The type of this value</typeparam>
        public void Set<T>(string key, T value)
        {
            if (!KEY_CONVERSIONS.ContainsKey(key))
            {
                Debug.LogError($"The given key '{key}' does not have a conversion function, so it isn't valid!");
                return;
            }
            
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        }

        /// <summary>
        /// Gets the value based on the identifying key
        /// </summary>
        /// <param name="key">The identifying key</param>
        /// <param name="defValue">The default value to return</param>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns>The data if conversion is successful and the key exists, default value otherwise</returns>
        public T Get<T>(string key, T defValue)
        {
            if (!data.ContainsKey(key))
            {
                Debug.LogWarning($"Trying to get key '{key}' but it's not present");
                return defValue;
            }
            
            try
            {
                return (T) data[key];
            }
            catch (Exception)
            {
                Debug.LogError($"The given key '{key}' cannot be converted for type '{typeof(T).Name}'");

                return defValue;
            }
        }

        /// <summary>
        /// Adds a conversion function for a data key
        /// </summary>
        /// <param name="key">The key to add the function to</param>
        /// <param name="convert">The conversion function</param>
        public static void AddKeyConversion(string key, SaveGame.ModdedData.ConversionFunction convert)
        {
            if (KEY_CONVERSIONS.ContainsKey(key))
            {
                Debug.LogError($"The given key '{key}' already has a conversion callback.");

                return;
            }
            
            KEY_CONVERSIONS.Add(key, convert);
        }
        
        //+ EVENT TRIGGERS
        // Triggers the Setup
        internal static void TriggerSetup()
        {
            if (KEY_CONVERSIONS.Count <= 0)
                Setup?.Invoke();
        }

        // Processes the data that got loaded
        internal void ProcessData()
        {
            ProcessLoad?.Invoke(this);
        }
    }
}