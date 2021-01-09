using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Guu.SaveGame
{
    ///<summary>Handles everything related to modded save data</summary>
    public static class ModdedSaveHandler
    {
        //+ VARIABLES
        // The list of all the modded data sorted by their model id
        private static readonly Dictionary<string, SaveSystem.ModdedData> MOD_DATA = new Dictionary<string, SaveSystem.ModdedData>();

        // The save data to be converted to and from a save file
        private static ModdedSaveData saveData = new ModdedSaveData();
        
        //+ DATA CONTROL
        // Registers a new modded data component into the save system
        internal static void RegisterModdedData(SaveSystem.ModdedData data, string id)
        {
            if (id.Equals(string.Empty)) return;
            
            PopulateData(id);
            
            if (!MOD_DATA.ContainsKey(id))
                MOD_DATA.Add(id, data);
            else
                Debug.Log($"Couldn't registered modded save data for ID '{id}' because it is already registered!");
        }

        // Populates all the data in the save data
        private static void PopulateData(string id)
        {
            // Sets up the Modded Data environment if it wasn't set up before
            SaveSystem.ModdedData.TriggerSetup();
            
            // Checks for the existence of modded save data
            if (saveData == null) return;
            
            // Loops the data to check what belongs where
            List<ModdedSaveData.DataStruct> toRemove = new List<ModdedSaveData.DataStruct>();
            
            Debug.Log("Test here + " + saveData.saveData);
            foreach (ModdedSaveData.DataStruct data in saveData.saveData.Where(data => MOD_DATA.ContainsKey(data.id) && data.id.Equals(id)))
            {
                toRemove.Add(data);
                
                Debug.Log("Test things");
                SaveSystem.ModdedData mData = MOD_DATA[data.id];
                mData.data.Add(data.key, SaveSystem.ModdedData.KEY_CONVERSIONS[data.key].load?.Invoke(data.value) ?? data.value);
                mData.ProcessData();
                Debug.Log("Test things after");
            }

            Debug.Log("Test there");
            // Deletes all the data already loaded
            saveData.saveData.RemoveAll(data => toRemove.Contains(data));
        }
        
        

        //+ SAVE/LOAD METHODS
        // Saves the game
        internal static void SaveGame(FileStorageProvider provider, string saveName)
        {
            Debug.Log("Save Game");
            
            // Ensures the modded save data exists
            if (saveData == null) saveData = new ModdedSaveData();

            // Gets the path to save to
            MethodInfo savePath = provider?.GetType().GetMethod("GetFullFilePath", new[] { typeof(string) });
            string path = savePath?.Invoke(provider, new object[] { saveName }) as string;

            path = path?.Replace($"{saveName}.sav", $"Guu/Saves/{saveName}.sav");
            
            // Generates the modded save data
            foreach (string id in MOD_DATA.Keys)
            {
                foreach (KeyValuePair<string, object> pair in MOD_DATA[id].data)
                {
                    saveData.saveData.Add(new ModdedSaveData.DataStruct
                    {
                        id = id,
                        key = pair.Key,
                        value = SaveSystem.ModdedData.KEY_CONVERSIONS[pair.Key].save?.Invoke(pair.Value) ??
                                pair.Value.ToString()
                    });
                }
            }
            
            // Saves the data to the file
            saveData.SaveFile(path);
            
            // Clears the mod data after save
            MOD_DATA.Clear();
        }

        // Loads the game
        internal static void LoadGame(FileStorageProvider provider, string saveName)
        {
            // Gets the path to load from
            MethodInfo savePath = provider?.GetType().GetMethod("GetFullFilePath", new[] { typeof(string) });
            string path = savePath?.Invoke(provider, new object[] { saveName }) as string;

            path = path?.Replace($"{saveName}.sav", $"Guu/Saves/{saveName}.sav");
            
            // Loads the data from the fil
            saveData = ModdedSaveData.LoadFile(path);
            
            // Clears Modded Data
            MOD_DATA.Clear();
        }

        // Deletes the game
        internal static void DeleteGame(FileStorageProvider provider, string saveName)
        {
            // Gets the path to load from
            MethodInfo savePath = provider?.GetType().GetMethod("GetFullFilePath", new[] { typeof(string) });
            string path = savePath?.Invoke(provider, new object[] { saveName }) as string;

            path = path?.Replace($"{saveName}.sav", $"Guu/Saves/{saveName}.sav");
            
            // Deletes the file
            if (path != null) File.Delete(path);
        }
    }
}