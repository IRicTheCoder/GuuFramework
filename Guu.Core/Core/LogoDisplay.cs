using System;
using System.Collections.Generic;
using System.Reflection;
using EdenUnity.Core.Code;
using UnityEngine;
using EventHandler = Guu.Services.Events.EventHandler;

namespace Guu
{
	internal class LogoDisplay : USingleton<LogoDisplay>
	{
		//+ VARIABLES
		private Rect rect = new Rect(0, 0, 64, 64);
		private Rect verRect = new Rect(0, 0, 0, 40);

		private bool showLogo;
		private Sprite logo;

		//+ BEHAVIOUR
		protected override void Awake()
		{
			base.Awake();
			logo = GuuCore.guiPack.Get<Sprite>("GuuLogo");
			EventHandler.OnApplyResolution += ResolutionChange;
		}

		private void Update()
		{
			if (Levels.IsLevel(Levels.WORLD) && showLogo) showLogo = false;
			if (!Levels.IsLevel(Levels.WORLD) && !showLogo) showLogo = true;
		}

		private void OnGUI()
		{
			if (!showLogo) return;

			if (verRect.width == 0)
				ResolutionChange();

			GUI.depth = int.MinValue;
			GUI.DrawTexture(rect, logo.texture, ScaleMode.ScaleToFit);
			GUI.Label(verRect, GuuCore.GUU_VERSION);
		}

		private void OnDestroy()
		{
			EventHandler.OnApplyResolution -= ResolutionChange;
		}

		//+ EVENTS
		private void ResolutionChange()
		{
			verRect.width = GUI.skin.label.CalcSize(new GUIContent(GuuCore.GUU_VERSION)).x;
			
			rect.x = 20 + (verRect.width / 2 - rect.width / 2);
			rect.y = Screen.height - rect.height - 40;
			
			verRect.x = 20;
			verRect.y = rect.y + rect.height;
		}
	}
}