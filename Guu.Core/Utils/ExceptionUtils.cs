using System;

namespace Guu.Utils
{
	/// <summary>
	/// An utility class to help with exceptions
	/// </summary>
	public static class ExceptionUtils
	{
		/// <summary>
		/// This is made to ignore exceptions so it doesn't fill the log
		/// with unneeded errors. As all of these errors are accounted for,
		/// beforehand.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="func">Function to execute</param>
		/// <returns>Object returned</returns>
		public static T IgnoreErrors<T>(Func<T> func)
		{
			if (func == null) return default;

			try { return func(); }
			catch { return default; }
		}

		/// <summary>
		/// This is made to ignore exceptions so it doesn't fill the log
		/// with unneeded errors. As all of these errors are accounted for,
		/// beforehand.
		/// </summary>
		/// <typeparam name="T">Type to return</typeparam>
		/// <param name="func">Function to execute</param>
		/// <param name="def">The default value to return</param>
		/// <returns>Object returned</returns>
		public static T IgnoreErrors<T>(Func<T> func, T def)
		{
			if (func == null) return def;

			try { return func(); }
			catch { return def; }
		}

		/// <summary>
		/// This is made to ignore exceptions so it doesn't fill the log
		/// with unneeded errors. As all of these errors are accounted for,
		/// beforehand.
		/// </summary>
		/// <param name="action">Action to execute</param>
		public static void IgnoreErrors(Action action)
		{
			if (action == null) return;

			try { action(); }
			catch
			{
				// ignored
			}
		}
	}
}
