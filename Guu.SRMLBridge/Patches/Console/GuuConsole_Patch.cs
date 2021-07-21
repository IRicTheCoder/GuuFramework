using Eden.Patching.Harmony;
using Guu.Console;

namespace Guu.SRMLBridge
{
	[EdenHarmony.Wrapper(typeof(GuuConsole))]
	internal static class GuuConsole_Patch
	{
		private static void InvokeCatchers_Postfix(ref bool @return, string cmd, string[] args)
		{
			if (!@return) return;

			@return = ConsoleBridge.RunCatchers(cmd, args);
		}
	}
}