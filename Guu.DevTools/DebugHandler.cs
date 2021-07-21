using EdenUnity.Core.Code;
using Guu.DevTools.DevMenu;
using Guu.Services;
using Guu.Services.Windows;
using UnityEngine;

namespace Guu.DevTools
{
	internal class DebugHandler : USingleton<DebugHandler>, IServiceInternal
	{
		internal static DevMenuWindow devWindow;

		protected override void Awake()
		{
			base.Awake();
			
			devWindow = new DevMenuWindow();
			WindowManager.RegisterWindow(devWindow);
		}
		
		private void Update()
		{
			// Opens the Dev Menu
			if (Input.GetKeyDown(KeyCode.F1))
				devWindow.Open();
		}
	}
}