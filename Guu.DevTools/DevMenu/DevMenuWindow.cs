using System.Collections.Generic;
using System.Linq;
using Guu.Windows;
using UnityEngine;

namespace Guu.DevTools.DevMenu
{
    /// <summary>
    /// Used to display the exception that crashed the game
    /// </summary>
    internal class DevMenuWindow : SystemWindow 
    {
        //+ CONSTANTS
        internal const string OTHERS = "Others";
        
        internal const int TAB_WIDTH = 120;

        //+ VARIABLES
        internal bool wasPaused;
        internal SRInput.InputMode previousInput;
        
        // Will contain the dev tabs available on the menu
        internal static readonly Dictionary<string, DevTab> DEV_TABS = new Dictionary<string, DevTab>();
        internal static readonly HashSet<SystemWindow> DEV_WINDOWS = new HashSet<SystemWindow>();
        
        internal static string currentTab;

        //+ PROPERTIES
        public override string Title => $"Development Menu - {DEV_TABS[currentTab].Title}";
        public override float MaxWidth => Screen.width * 0.80f;
        
        //+ CONSTRUCTOR
        internal DevMenuWindow() : base("devMenu")
        {
            RegisterTab(new ConsoleTab());
            // TODO: Finish Dev menu tabs
            RegisterTab(new InspectorTab());
            //RegisterTab(new CheatMenuTab());

            currentTab = DEV_TABS.Keys.First();
        }
        
        //+ REGISTRATION
        internal static DevTab RegisterTab(DevTab tab)
        {
            if (DEV_TABS.ContainsKey(tab.ID))
            {
                GuuCore.LOGGER.LogWarning($"Trying to register a dev tab with id '{tab.ID}' but the ID is already registered!");
                return null;
            }
            
            DEV_TABS.Add(tab.ID, tab);
            return tab;
        }

        /// <summary>
        /// Registers a new window into the dev menu
        /// </summary>
        /// <param name="window">The window to register</param>
        /// <returns>True if the window was registered, false otherwise</returns>
        public static bool RegisterWindow(SystemWindow window) => DEV_WINDOWS.Add(window);

        //+ ACTIONS
        // Triggers when the window opens
        protected override void OnOpen()
        {
            //& Dev menu can only run on the menu and on the world, anywhere else it should be denied
            if (!Levels.isMainMenu() || Levels.IsLevel(Levels.WORLD))
            {
                Close();
                return;
            }
            
            if (!SceneContext.Instance.TimeDirector.HasPauser())
                SceneContext.Instance.TimeDirector.Pause(true, true);
            else
                wasPaused = true;

            previousInput = SRInput.Instance.GetInputMode();
            SRInput.Instance.SetInputMode(SRInput.InputMode.NONE);
            
            DEV_TABS[currentTab].Show();
        }
        
        // Triggers when the window close
        protected override void OnClose()
        {
            if (!wasPaused)
                SceneContext.Instance.TimeDirector.Unpause();
            
            SRInput.Instance.SetInputMode(previousInput);
            wasPaused = false;
            
            DEV_TABS[currentTab].Hide();
        }

        // Triggers when the window updates
        public override void OnUpdate(bool enabled)
        {
            //& Dev menu can only run on the menu and on the world, anywhere else it should be denied
            // Added in the Update too to make sure it doesn't run
            if (!Levels.isMainMenu() || Levels.IsLevel(Levels.WORLD))
            {
                Close();
                return;
            }
            
            //& Tab Update
            DEV_TABS[currentTab].OnUpdate();
        }

        //+ DISPLAY
        // Draws the window
        public override void DrawWindow()
        {
            //& Process the input for the window
            if (ProcessInput())
            {
                Event.current.Use();
                return;
            }
            
            //& Draws the tabs for this menu
            GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.Height(25));

            foreach (DevTab tab in DEV_TABS.Values)
            {
                bool isCurrent = currentTab.Equals(tab.ID);
                if (isCurrent) GUI.backgroundColor = Color.red;
                if (GUILayout.Button(isCurrent ? $"<b>{tab.Title}</b>" : tab.Title, GUILayout.Width(TAB_WIDTH), GUILayout.ExpandWidth(false)))
                {
                    DEV_TABS[currentTab].Hide();
                    currentTab = tab.ID;
                    tab.Show();
                }
                GUILayout.Space(5);
                
                GUI.backgroundColor = Color.white;
            }
            GUILayout.FlexibleSpace();
            GUILayout.Button(OTHERS, GUILayout.Width(TAB_WIDTH), GUILayout.ExpandWidth(false));
            
            GUILayout.EndHorizontal();

            //& Draws the current selected tab
            GUILayout.BeginHorizontal(GUIStyle.none, GUILayout.ExpandHeight(true));
            DEV_TABS[currentTab].DrawTab();
            GUILayout.EndHorizontal();
        }
        
        //+ INPUT CONTROL
        private static bool ProcessInput()
        {
            // Prevents input from running
            if (!Event.current.isKey || Event.current.type != EventType.KeyDown) return false;
            
            EventModifiers mods = Event.current.modifiers;

            // Runs the tab's input first and if nothing happens runs the window input
            if (DEV_TABS[currentTab].OnProcessInput(mods)) return true;
            
            switch (Event.current.keyCode)
            {
                // Closes the menu if ESC is pressed 
                case KeyCode.Escape:
                    DebugHandler.devWindow.Close();
                    Input.ResetInputAxes();

                    return true;
                    
                // Closes the menu if F1 is pressed
                case KeyCode.F1:
                    DebugHandler.devWindow.Close();
                    Input.ResetInputAxes();

                    return true;
                
                // Anything else
                default:
                    return false;
            }
        }
    }
}