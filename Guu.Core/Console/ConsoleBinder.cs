using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

namespace Guu.Console
{
	/// <summary>Controls the user bindings for the console</summary>
	public static class ConsoleBinder
	{
		//+ CONSTANTS
		internal static readonly string BIND_FILE = Path.Combine(GuuCore.BINDINGS_FOLDER, "consoleButtons.bindings");
		
		//+ MANIPULATION
		internal static void ReadBinds()
		{
			if (!File.Exists(BIND_FILE)) return;

			foreach (string line in File.ReadAllLines(BIND_FILE))
			{
				if (!line.Contains(":"))
					continue;

				string[] split = line.Split(':');
				GuuConsole.RegisterButton(new ConsoleButton($"{split[0]}", split[1], split[2], true, true));
			}
		}
		
		internal static void RegisterBind(ConsoleButton button)
		{
			File.AppendAllText(BIND_FILE, $"{button}\n");
		}

		internal static void RemoveBind(string id)
		{
			File.WriteAllText(BIND_FILE, File.ReadAllText(BIND_FILE).Replace(GuuConsole.customButtons[id].ToString(), string.Empty));
		}
		
		internal static void EditBind(ConsoleButton button)
		{
			File.WriteAllText(BIND_FILE, File.ReadAllText(BIND_FILE).Replace(GuuConsole.customButtons[button.ID].ToString(), button.ToString()));
		}
	}
}