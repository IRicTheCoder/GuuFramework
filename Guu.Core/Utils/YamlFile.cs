using System;
using System.Collections.Generic;
using System.IO;
using Eden.Core.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// ReSharper disable UnusedMember.Global
namespace Guu.Utils
{
    /// <summary>
    /// Represents a YamlFile for easy use, already has all the methods required to avoid the need to make them
    /// each time a new file structure is needed.
    /// </summary>
    public class YamlFile<T>
    {
        //+ PROPERTIES
        public FileInfo Info { get; }

        //+ CONSTRUCTOR
        public YamlFile(string path) : this(new FileInfo(path)) { }

        public YamlFile(FileInfo file)
        {
            Info = file;
        }

        //+ FILE PROCESS
        public T Read()
        {
            if (!Info.Exists) return default;

            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            
            return deserializer.Deserialize<T>(File.ReadAllText(Info.FullName));
        }

        internal void Write(T instance)
        {
            if (Info.Exists)
                Info.Delete();

            using (StreamWriter writer = Info.CreateText())
            {
                ISerializer serializer = new SerializerBuilder().Build();
                
                writer.Write(serializer.Serialize(instance));
                writer.Flush();
            }
        }
    }
}