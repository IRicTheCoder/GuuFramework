using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Guu
{
	/// <summary>Used to handle all API processes throughout the load of API content</summary>
	public static class APIHandler
	{
		//+ CONSTANTS
		private const string PREFAB_OBJECT_NAME = "_PrefabHolder";
		
		//+ EVENTS
		/// <summary>Triggers when the registration is finished and needs to be handled</summary>
		internal static event Action HandleRegistration;

		/// <summary>Triggers when the loading process is done and the Handle step is about to be called</summary>
		internal static event Action HandleItems;

		/// <summary></summary>
		internal static event Action ClearMemory;
		
		//+ PROPERTIES
		/// <summary>The object that holds all prefabs created by Guu</summary>
		public static GameObject PrefabHolder { get; private set; }
		
		//+ INITIALIZATION
		internal static void InitializeHandler()
		{
			PrefabHolder = new GameObject(PREFAB_OBJECT_NAME);
			Object.DontDestroyOnLoad(PrefabHolder);
			PrefabHolder.SetActive(false);

			GuuCore.LOGGER?.Log("- Created the object to hold prefabs");
		}

		//+ REGISTRY HANDLING
		internal static void OnHandleRegistration()
		{
			HandleRegistration?.Invoke();
			HandleRegistration = null;
		}
		
		internal static void OnHandleItems()
		{
			HandleItems?.Invoke();
			HandleItems = null;
		}
		
		internal static void OnClearMemory()
		{
			ClearMemory?.Invoke();
			ClearMemory = null;
		}
	}
}