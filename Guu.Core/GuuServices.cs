using System;
using System.Linq;
using Eden.Core.Utils;
using Guu.Services;
using Guu.Services.Windows;
using UnityEngine;
using EventHandler = Guu.Services.Events.EventHandler;
using Object = UnityEngine.Object;

namespace Guu
{
	/// <summary>This class controls all services that guu runs, use it to activate the services your mod needs.</summary>
	public static partial class GuuServices
	{
		//+ CONSTANTS
		private const string SYSTEM_OBJECT_NAME = "_GuuServices";
		
		//+ VARIABLES
		internal static bool internalServicesInit;
		internal static GameObject servicesObj;

		//+ INITIALIZATION
		internal static void InitInternalServices()
		{
			// Prevents the services from being initialized again
			if (internalServicesInit) return;
			
			//& Registering all services
			foreach (Type type in TypeUtils.GetChildsOf<IService>(GuuCore.MAIN_ASSEMBLIES.ToArray()))
			{
				if (!type.IsSubclassOf(typeof(Component)) || servicesObj.HasComponent(type)) continue;

				if (typeof(IServiceInternal).IsAssignableFrom(type))
				{
					servicesObj.AddComponent(type);
					GuuCore.LOGGER?.Log($"- Registered internal service '{type.Name}'");
					continue;
				}

				Component comp = servicesObj.AddComponent(type);
				if (comp is Behaviour behaviour) behaviour.enabled = false;
				GuuCore.LOGGER?.Log($"- Registered service '{type.Name}'");
			}

			// Marks the services as initialized
			internalServicesInit = true;
		}

		internal static void CreateServiceObject()
		{
			//& Sets up the game object for Guu's services
			servicesObj = new GameObject(SYSTEM_OBJECT_NAME, typeof(LogoDisplay));
			Object.DontDestroyOnLoad(servicesObj);
			
			GuuCore.LOGGER?.Log($"- Created the services GameObject");
		}
	}
}