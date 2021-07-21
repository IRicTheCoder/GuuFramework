using System.Collections.Generic;
using Eden.Core.Utils;
using EdenUnity.Core.Code;
using Guu.Windows;
using UnityEngine;

namespace Guu.Services.Windows
{
    /// <summary>This class is used to display system windows, so that all windows are centralized</summary>
    internal class WindowHandler : USingleton<WindowHandler>, IServiceInternal
    {
        //+ VARIABLES
        internal static List<string> windowIDs = new List<string>();
        internal static List<string> toClose = new List<string>();
        internal static List<string> toOpen = new List<string>();
        internal static string lastID;

        internal static Rect screenRect;

        //+ BEHAVIOUR
        protected override void Awake()
        {
            base.Awake();
            
            WindowManager.Skin = GuuCore.guiPack.Get<GUISkin>("guiSkin");
            screenRect = new Rect(0, 0, Screen.width, Screen.height);
        }

        private void OnGUI()
        {
            if (windowIDs.Count == 0) return;
                
            GUISkin old = GUI.skin;
            GUI.skin = WindowManager.Skin;

            GUI.depth = WindowManager.START_DEPTH;
            GUI.color = Color.black.Alpha(0.5f);
            GUI.Box(screenRect, string.Empty, GUI.skin.GetStyle("background"));
            GUI.color = Color.white;
            GUI.depth--;
            
            foreach (string id in windowIDs)
            {
                SystemWindow window = WindowManager.WINDOWS[id];

                EventType current = EventType.Ignore;
                if (!id.Equals(lastID))
                {
                    current = Event.current.type;
                    if (current != EventType.Layout && current != EventType.Repaint && current != EventType.Used)
                        Event.current.type = EventType.Ignore;
                    
                    GUI.color = Color.gray;
                }
                    
                GUILayout.BeginArea(window.Rect, window.Title, GUI.skin.window);
                window.Draw();
                GUILayout.EndArea();
                window.DrawUnbound();
                
                GUI.depth--;
                
                GUI.color = Color.white;
                if (current != EventType.Ignore) Event.current.type = current;
            }

            GUI.skin = old;
        }

        private void Update()
        {
            if (toOpen.Count > 0)
            {
                windowIDs.AddRange(toOpen);
                lastID = toOpen[toOpen.Count - 1];
                
                toOpen.Clear();
            }

            if (toClose.Count > 0)
            {
                windowIDs.RemoveAll(id => toClose.Contains(id));
                toClose.Clear();
                
                if (!windowIDs.Contains(lastID) && windowIDs.Count > 0) lastID = windowIDs[windowIDs.Count - 1];
            }
            

            if (windowIDs.Count == 0) return;
            
            foreach (string id in windowIDs)
            {
                SystemWindow window = WindowManager.WINDOWS[id];
                window.OnUpdate(id.Equals(lastID));
            }
        }
    }
}