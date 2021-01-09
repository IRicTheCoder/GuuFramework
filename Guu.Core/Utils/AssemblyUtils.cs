using System.Diagnostics;
using System.Reflection;

namespace Guu.Utils
{
	/// <summary>
	/// An utility class to help with Assembly things
	/// </summary>
	public static class AssemblyUtils
	{
		/// <summary>
		/// Gets the Relevant assembly by checking the type given
		/// </summary>
		/// <param name="type">The type to get the assembly from</param>
		/// <returns>The relevant assembly to the context</returns>
		public static Assembly GetRelevant(object type)
		{
			return type.GetType().Assembly;
		}

		/// <summary>
		/// Gets the Relevant assembly by tracing the call
		/// </summary>
		/// <returns>The relevant assembly to the context</returns>
		public static Assembly GetRelevant()
		{
			StackTrace trace = new StackTrace();
			Assembly calling = Assembly.GetCallingAssembly();

			StackFrame[] frames = trace.GetFrames();

			return ExceptionUtils.IgnoreErrors(() =>
			{
				if (frames == null) return calling;
				
				foreach (StackFrame frame in frames)
				{
					Assembly assembly = frame.GetMethod().DeclaringType?.Assembly;

					if (assembly != calling && assembly != null) return assembly;
				}

				return calling;
			}, calling);
		}
	}
}
