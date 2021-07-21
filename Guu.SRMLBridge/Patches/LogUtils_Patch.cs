using Eden.Patching.Harmony;
using SRML.Utils;

namespace Guu.SRMLBridge
{
	[EdenHarmony.Wrapper(typeof(LogUtils))]
	internal static class LogUtils_Patch
	{
		private static bool OpenLogSession_Prefix() => false;
		private static bool Log_Prefix() => false;
		private static bool CloseLogSession_Prefix() => false;
	}
}