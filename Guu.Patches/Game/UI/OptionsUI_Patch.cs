using Eden.Patching.Harmony;
using JetBrains.Annotations;
using EventHandler = Guu.Services.Events.EventHandler;

namespace Guu.Patches.Game
{
	[EdenHarmony.Wrapper(typeof(OptionsUI))]
	[UsedImplicitly]
	internal static class OptionsUI_Patch
	{
		[UsedImplicitly] private static void OnAudioLevelsChanged_Postfix(OptionsUI @this) => EventHandler.Instance.OnAudioLevelsChanged_Trigger(@this);
		[UsedImplicitly] private static void OnApplyResolution_Postfix(OptionsUI @this) => EventHandler.Instance.OnApplyResolution_Trigger(@this);
	}
}