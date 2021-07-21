using System.Collections.Generic;
using System.IO;
using Eden.Patching.Harmony;
using Guu.Game;
using UnityEngine;

namespace Guu.Patches.Language
{
	[EdenHarmony.Wrapper(typeof(ResourceBundle))]
	internal static class ResourceBundle_Patch
	{
		private static void LoadFromText_Postfix(Dictionary<string, string> @return, string path, string text)
		{
			LanguageController.ResetTranslations(GameContext.Instance.MessageDirector);

			if (!LanguageController.TRANSLATIONS.ContainsKey(path) && GuuCore.DEBUG)
			{
				GuuCore.LOGGER.Log($"Missing bundle in translations: {path}");
				FileInfo file = new FileInfo($"{Application.dataPath}/{path}.yaml");

				using (StreamWriter writer = file.CreateText())
				{
					writer.WriteLine("#=====================================");
					writer.WriteLine("# AUTO GENERATED FROM THE GAME");
					writer.WriteLine("#=====================================");
					writer.WriteLine("");

					foreach (string key in @return.Keys)
						writer.WriteLine($"{path}:{key}: \"{@return[key].Replace("\"", "\\\"").Replace("\n", "\\n")}\"");
				}

				return;
			}

			foreach (KeyValuePair<string, string> pair in LanguageController.TRANSLATIONS[path])
			{
				if (@return.ContainsKey(pair.Key)) @return[pair.Key] = pair.Value;
				else @return.Add(pair.Key, pair.Value);
			}
		}
	}
}