using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Guu.Loader;
using UnityEngine;

namespace Guu.Utils
{
    /// <summary>
    /// This class helps handle the read and write of .yaml files.
    /// </summary>
    public static class YamlUtils
    {
        //+ INTERNALS
        internal static YamlFile<GuuMod>[] GetModFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(GuuCore.MODS_FOLDER);

            if (!dir.Exists)
            {
                dir.Create();
                return new YamlFile<GuuMod>[0];
            }

            List<YamlFile<GuuMod>> files = new List<YamlFile<GuuMod>>();
            foreach (DirectoryInfo subdir in dir.EnumerateDirectories())
            {
                FileInfo[] found = subdir.GetFiles(GuuCore.MODINFO_FILE, SearchOption.TopDirectoryOnly);

                if (found.Length > 1)
                {
                    Debug.LogError($"Trying to load {GuuCore.MODINFO_FILE} in directory {subdir.Name}, but found multiple. Skipping mod!");
                    continue;
                }
                
                if (found.Length > 0)
                    files.Add(new YamlFile<GuuMod>(found[0]));
            }

            return files.ToArray();
        }
    }
}