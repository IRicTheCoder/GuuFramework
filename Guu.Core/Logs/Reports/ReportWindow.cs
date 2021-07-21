using System.Diagnostics;
using System.IO;
using Guu.Windows;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Guu.Logs
{
    /// <summary>
    /// Used to display the exception that crashed the game
    /// </summary>
    internal class ReportWindow : SystemWindow 
    {
        //+ CONSTANTS
        private const string GUU_LOG = "Open Guu's Log";
        private const string UNITY_LOG = "Open Unity's Log";
        private const string COPY = "Copy Text";
        private const string OPEN = "Open Report Folder";
        private const string CLOSE = "Close";

        private const int BUTTON_MIN_WIDTH = 100;
        
        //+ VARIABLES
        private static Vector2 scrollPosition;
        
        internal static CrashReport report;

        //+ PROPERTIES
        public override string Title => "Crash Report";
        public override float MaxWidth => Screen.width * 0.6f;
        
        //+ CONSTRUCTOR
        internal ReportWindow() : base("reportWindow") { }

        //+ ACTIONS
        protected override void OnOpen()
        {
            report = new CrashReport(InternalLogger.handledException);
            scrollPosition = Vector2.zero;
        }
        
        protected override void OnClose()
        {
            InternalLogger.handledException = null;
            
            if (!GuuCore.CMD_ARGS.Contains("--guuSilent") && !GuuCore.DEBUG && GuuCore.LAUNCHER_EXE.Exists) 
                Process.Start(GuuCore.LAUNCHER_EXE.FullName);
            
            GuuCore.GAME_PROCESS.Kill();
        }

        public override void OnUpdate(bool enabled)
        {
            // Check if the game is paused and if not pause it
            if (SceneContext.Instance == null || !(Time.timeScale > 0)) return;

            SRInput.Instance?.SetInputMode(SRInput.InputMode.NONE);
            SceneContext.Instance.TimeDirector.Pause(true, true);
            Object.FindObjectOfType<MainMenuUI>()?.gameObject.SetActive(false);
            GameObject.Find("UMF Root")?.SetActive(false);
            Time.timeScale = 0;
        }

        //+ DISPLAY
        public override void DrawWindow()
        {
            if (InternalLogger.handledException == null)
            {
                Close();
                return;
            }
            
            //& Display report
            // Draw Message
            GUILayout.BeginVertical(GUI.skin.textArea);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
            GUILayout.Label(report?.DisplayMessage, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            //& Draw Buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GUU_LOG, GUILayout.MinWidth(BUTTON_MIN_WIDTH))) Process.Start(InternalLogger.NEW_UNITY_LOG_FILE);
            GUILayout.Space(5);
            if (GUILayout.Button(UNITY_LOG, GUILayout.MinWidth(BUTTON_MIN_WIDTH))) Process.Start(Path.Combine(GuuCore.GUU_FOLDER, GuuCore.LOG_FILE));
            GUILayout.Space(5);
            if (GUILayout.Button(COPY, GUILayout.MinWidth(BUTTON_MIN_WIDTH))) GUIUtility.systemCopyBuffer = report?.DisplayMessage;
            GUILayout.Space(5);
            if (GUILayout.Button(OPEN, GUILayout.MinWidth(BUTTON_MIN_WIDTH))) Process.Start($"explorer.exe {GuuCore.REPORT_FOLDER}");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CLOSE, GUILayout.MinWidth(BUTTON_MIN_WIDTH))) Close();
            GUILayout.EndHorizontal();
        }
    }
}