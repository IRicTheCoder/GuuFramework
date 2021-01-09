using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Guu.SaveGame
{
    ///<summary>Contains all the data of a modded save</summary>
    [XmlRoot("ModdedSaveData")]
    public class ModdedSaveData
    {
        //+ DATA STRUCTURE
        ///<summary>The structure to contain save data</summary>
        public struct DataStruct
        {
            [XmlElement("ModelID")] public string id;
            [XmlElement("Key")] public string key;
            [XmlElement("Value")] public string value;
        }
        
        //+ DATA LIST
        ///<summary>The data to be saved</summary>
        [XmlArray("SaveData"), XmlArrayItem("Data")]
        public List<DataStruct> saveData = new List<DataStruct>();
        
        //+ FILE CONTROL
        /// <summary>
        /// Saves the save file to the disk
        /// </summary>
        /// <param name="path">The path to save to</param>
        public void SaveFile(string path)
        {
            if (path == null) return;

            // ReSharper disable once AssignNullToNotNullAttribute
            if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (!File.Exists(path)) File.Create(path).Close();

            using (StreamWriter writer = new StreamWriter(path))
            {
                XmlSerializer ser = new XmlSerializer(GetType());
                ser.Serialize(writer, this);
                writer.Flush();
            }
        }

        /// <summary>
        /// Loads a save file from disk
        /// </summary>
        /// <param name="path">The path to load from</param>
        /// <returns>The ModdedSaveData object</returns>
        public static ModdedSaveData LoadFile(string path)
        {
            if (path == null) return null;

            if (!File.Exists(path)) return null;
            
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ModdedSaveData));
                return ser.Deserialize(stream) as ModdedSaveData;
            }
        }
    }
}