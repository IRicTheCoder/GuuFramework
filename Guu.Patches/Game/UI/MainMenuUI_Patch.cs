using Eden.Patching.Harmony;
using Guu.Game;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(MainMenuUI))]
	[UsedImplicitly]
	internal static class MainMenuUI_Patch
	{
		[UsedImplicitly]
		private static void Start_Postfix()
		{
			foreach (TMP_Text text in Resources.FindObjectsOfTypeAll<TMP_Text>())
			{
				if (text.HasComponent<TMP_Dropdown>()) continue;
				if (!text.HasComponent<CustomLangSupport>()) text.gameObject.AddComponent<CustomLangSupport>();
			}
		}
	}
}