using UnityEngine;

namespace Guu.API
{
	/// <summary>A class that contains some utility methods for the API system</summary>
	public class APIUtils
	{
		/// <summary>A Prefab built by the API</summary>
		public class APIPrefab : Component
		{
			public IBuilder apiBuilder { get; internal set; }
		}
	}
}