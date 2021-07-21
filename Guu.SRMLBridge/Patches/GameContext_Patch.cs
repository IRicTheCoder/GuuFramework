using System;
using System.IO;
using System.Reflection;
using Eden.Core.Utils;
using Eden.Patching.Harmony;
using Guu.Logs;
using HarmonyLib;

namespace Guu.SRMLBridge
{
	[EdenHarmony.Wrapper(typeof(GameContext), int.MaxValue)]
	public class GameContext_Patch
	{
		private static bool LoadSRModLoader_Prefix()
		{
			try
			{
				foreach (string file in Directory.GetFiles("SRML/Libs", "*.dll", SearchOption.AllDirectories))
					Assembly.LoadFrom(file);
				TypeUtils.GetTypeBySearch("SRML.Main").GetMethod("PreLoad", AccessTools.all)?.Invoke(null, null);
			}
			catch (Exception ex)
			{
				InternalLogger.HandleException(ex is TargetInvocationException te ? te.InnerException : ex, ConsoleBridge.SRML_LOGGER);
				return false;
			}
			
			return false;
		}
	}
}