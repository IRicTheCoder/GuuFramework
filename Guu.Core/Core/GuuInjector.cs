using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Eden.Core.Utils;
using Eden.Patching.Harmony;
using Guu.Logs;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace Guu
{
	// Contains the Injection Code and Required patches for the injection procedure
	internal static class GuuInjector
	{
		// Injects Guu into the game
		internal static void InjectGuu()
		{
			EdenHarmony harmony = new EdenHarmony("<internal>Guu");
			harmony.PatchWrapper(typeof(GameContext_Patch));
		}

		// A patch to allow the patching of the GameContext.Awake method
		[EdenHarmony.Wrapper(typeof(GameContext))]
		[EdenHarmony.Skip, UsedImplicitly]
		internal static class GameContext_Patch
		{
			private static void Awake_Prefix()
			{
				GuuCore.LOGGER.Log("Guu has been successfully loaded by the game!");
				InternalLogger.HandleThrow(GuuCore.LoadGuu);
			}

			private static void Awake_Postfix()
			{
				GuuCore.LOGGER.Log("Executing Guu's late patches");
				GuuCore.harmony.ExecuteLatePatches();
			}
		}
	}
}