using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Guu.DevTools.DevMenu
{
	/// <summary>
	/// The tab that contains the Scene Inspector which allows for viewing a scene's
	/// hierarchy as well as the components of each game object.
	/// </summary>
	public class InspectorTab : DevTab
	{
		//+ CONSTANTS
		private const string OBJ_INSPECTOR_NAME = "<size=16><b>Inspecting - {name}</b></size>";
		private const string DONT_DESTROY_SCENE = "DontDestroyOnLoad";
		private const string PREFABS_SCENE = "Prefabs";
		private const string RELOAD_SCENE = "Reload Scene";
		
		internal const string FOLDED = "►";
		internal const string UNFOLDED = "▼";
		
		private const string PREVIOUS = "◄";
		private const string NEXT = "►";
		
		private const string HIDE = "H";
		private const string SHOW = "S";

		private const string SEARCH = "Search";
		private const string SHOW_HIDDEN = "Show Hidden";
		private const string SHOW_DISABLED = "Show Disabled";

		private const int TAB_SIZE = 16;

		//+ VARIABLES
		// Inspector Structures
		private static Vector2 hierarchyScroll = Vector2.zero;
		private static Vector2 inspectorScroll = Vector2.zero;
		
		// Hierarchy Caching
		private static int selectedScene;

		private static string searchFilter;
		private static bool showDisabled;
		private static bool showHidden;
		
		private static GameObject selectObject;
		
		private static readonly List<Scene> ALL_SCENES = new List<Scene>();
		private static readonly List<SceneHierarchyObject> SCENE_HIERARCHY = new List<SceneHierarchyObject>();
		private static readonly List<ObjectComponent> OBJECT_COMPONENTS = new List<ObjectComponent>();

		//+ PROPERTIES
		/// <summary>The title of this tab</summary>
		public override string Title => "Inspector";
		
		//+ CONSTRUCTOR
		/// <summary>Creates a new console tab</summary>
		public InspectorTab() : base("inspectorTab") { }
		
		//+ ACTIONS
		internal override void OnShow()
		{
			searchFilter = string.Empty;
			for (int i = 0; i < SceneManager.sceneCount; i++)
				ALL_SCENES.Add(SceneManager.GetSceneAt(i));
			
			selectedScene = ALL_SCENES.IndexOf(SceneManager.GetActiveScene());
			FindSceneObject();
		}

		internal override void OnHide()
		{
			SCENE_HIERARCHY.Clear();
			OBJECT_COMPONENTS.Clear();
			selectObject = null;
		}

		internal override void OnUpdate()
		{
			
		}

		//+ DISPLAY
		internal override void DrawTab()
		{
			//& Draws the hierarchy side
			GUILayout.BeginVertical(GUILayout.Width(DebugHandler.devWindow.Rect.width * 0.4f));
			DrawHierarchy(GUILayout.Width(DebugHandler.devWindow.Rect.width * 0.37f));
			GUILayout.EndVertical();
			
			//& Draws the inspector side
			// Fixes the weird margins
			GUIStyle.none.margin = new RectOffset(0, 0, 4, 4);
			
			// Draws the menu panel
			GUILayout.BeginVertical();
			DrawComponents();
			GUILayout.EndVertical();
		}

		private static void DrawHierarchy(GUILayoutOption width)
		{
			//& Draws the scene selector and filter
			// Scene Selector
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(PREVIOUS, GUILayout.ExpandWidth(false)))
			{
				selectedScene = selectedScene == -2 ? ALL_SCENES.Count - 1 : selectedScene - 1;
				FindSceneObject();
			}

			GUILayout.Label(selectedScene == -1 ? DONT_DESTROY_SCENE : 
							selectedScene == -2 ? PREFABS_SCENE : ALL_SCENES[selectedScene].name, GUI.skin.textField, GUILayout.ExpandWidth(true));
			
			if (GUILayout.Button(NEXT, GUILayout.ExpandWidth(false)))
			{
				selectedScene = selectedScene == ALL_SCENES.Count - 1 ? -2 : selectedScene + 1;
				FindSceneObject();
			}
			GUILayout.EndHorizontal();
			
			//& Filter
			// Filter Buttons
			GUILayout.BeginHorizontal();
			
			GUI.backgroundColor = showHidden ? Color.cyan : Color.white;
			if (GUILayout.Button(SHOW_HIDDEN)) showHidden = !showHidden;
			GUI.backgroundColor = Color.white;
			
			GUI.backgroundColor = showDisabled ? Color.cyan : Color.white;
			if (GUILayout.Button(SHOW_DISABLED)) showDisabled = !showDisabled;
			GUI.backgroundColor = Color.white;
			
			GUILayout.EndHorizontal();
			
			// Filter Box
			GUILayout.BeginHorizontal();

			searchFilter = GUILayout.TextField(searchFilter, GUILayout.ExpandWidth(true));
			if (GUILayout.Button(SEARCH, GUILayout.ExpandWidth(false))) FilterObjects();

			GUILayout.EndHorizontal();
			
			//& Draws the Hierarchy view
			GUILayout.BeginVertical(GUI.skin.textArea);
			hierarchyScroll = GUILayout.BeginScrollView(hierarchyScroll, false, true);

			// Creates the overflow button
			GUIStyle overflowButton = new GUIStyle(GUI.skin.textField)
			{
				clipping = TextClipping.Clip,
				wordWrap = false,
				alignment = TextAnchor.MiddleLeft,
				richText = true,
				hover =
				{
					textColor = Color.cyan
				}
			};

			// Lists all scene objects
			List<SceneHierarchyObject> markToDelete = new List<SceneHierarchyObject>();
			foreach (SceneHierarchyObject hierarchyObject in SCENE_HIERARCHY)
			{
				if (hierarchyObject.GameObject == null)
				{
					markToDelete.Add(hierarchyObject);
					continue;
				}
				
				if (!hierarchyObject.GameObject.activeSelf && !showDisabled) continue;
				if (hierarchyObject.IsHidden && !showHidden) continue;
				if (!CheckUnfolded(hierarchyObject)) continue;
				
				overflowButton.normal.textColor = selectObject == hierarchyObject.GameObject ? Color.yellow : hierarchyObject.GameObject.activeSelf ? Color.white : Color.gray;
				bool unfolded = hierarchyObject.IsUnfolded;

				GUILayout.BeginHorizontal(width);
				if (hierarchyObject.HasChildren())
				{
					GUILayout.Space(TAB_SIZE * hierarchyObject.ChildIndent);
					if (GUILayout.Button(unfolded ? UNFOLDED : FOLDED, overflowButton, GUILayout.ExpandWidth(false))) hierarchyObject.IsUnfolded = !unfolded;

					string name = hierarchyObject.GameObject.activeSelf ? hierarchyObject.GameObject.name : $"<i>{hierarchyObject.GameObject.name}</i>";
					if (GUILayout.Button(name, overflowButton, GUILayout.ExpandWidth(true))) SelectObject(hierarchyObject.GameObject);
				}
				else
				{
					GUILayout.Space(TAB_SIZE * hierarchyObject.ChildIndent);
					if (GUILayout.Button(hierarchyObject.GameObject.name, overflowButton)) SelectObject(hierarchyObject.GameObject);
				}

				if (GUILayout.Button(hierarchyObject.IsHidden ? SHOW : HIDE, GUI.skin.textField, GUILayout.ExpandWidth(false))) hierarchyObject.IsHidden = !hierarchyObject.IsHidden;
				GUILayout.EndHorizontal();
			}

			SCENE_HIERARCHY.RemoveAll(s => markToDelete.Contains(s));
			
			// Ends the hierarchy view
			GUI.color = Color.white;
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			
			//& Adds the button to reload the scene
			if (GUILayout.Button(RELOAD_SCENE)) FindSceneObject();
		}

		private static void DrawComponents()
		{
			//& Draws the Component view
			GUILayout.BeginVertical(GUI.skin.textArea);
			GUILayout.Label(OBJ_INSPECTOR_NAME.Replace("{name}", selectObject?.name ?? "Nothing"), GUI.skin.GetStyle("centerLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(20));
			inspectorScroll = GUILayout.BeginScrollView(inspectorScroll, false, true);

			// Creates the overflow button
			GUIStyle overflowButton = new GUIStyle(GUI.skin.textField)
			{
				clipping = TextClipping.Clip,
				wordWrap = false,
				alignment = TextAnchor.MiddleLeft,
				richText = true,
				hover =
				{
					textColor = Color.cyan
				}
			};
			
			foreach (ObjectComponent component in OBJECT_COMPONENTS)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(component.IsUnfolded ? UNFOLDED : FOLDED, GUI.skin.textField, GUILayout.ExpandWidth(false))) component.IsUnfolded = !component.IsUnfolded;
				
				if (component.Component is Behaviour behaviour)
				{
					bool enabled = behaviour.enabled;
					GUI.backgroundColor = enabled ? Color.yellow : Color.white;
					GUILayout.Label(string.Empty, overflowButton, GUILayout.ExpandWidth(false), GUILayout.Width(20));
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUILayout.Space(24);
				}

				GUILayout.Label(component.Component.GetType().Name, overflowButton, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				
				if (!component.IsUnfolded) continue;
				
				// TODO: Fix Component Inspection
				GUILayout.BeginVertical(GUI.skin.box);
				component.Inspector.DrawInspector();
				GUILayout.EndVertical();
			}
			
			// Ends the hierarchy view
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}

		//+ HELPERS
		private static void FindSceneObject()
		{
			SCENE_HIERARCHY.Clear();
			IEnumerable<GameObject> objects = selectedScene == -2
				? Resources.FindObjectsOfTypeAll<GameObject>().Where(o => o.transform.parent == null && o.scene.name == null)
				: Resources.FindObjectsOfTypeAll<GameObject>().Where(o => (o.scene.name?.Equals(selectedScene == -1 ? DONT_DESTROY_SCENE : ALL_SCENES[selectedScene].name) ?? false) && o.transform.parent == null);
			
			foreach (GameObject @object in objects)
			{
				SceneHierarchyObject current = new SceneHierarchyObject(@object, 0);
				SCENE_HIERARCHY.Add(current);
				if (@object.transform.childCount > 0) FindSceneObject(1, @object.transform, current);
			}

			if (SCENE_HIERARCHY.Count > 0)
				SelectObject(SCENE_HIERARCHY[0].GameObject);
		}
		
		// ReSharper disable once SuggestBaseTypeForParameter
		private static void FindSceneObject(int indent, Transform objects, SceneHierarchyObject parent)
		{
			foreach (Transform @object in objects)
			{
				SceneHierarchyObject current = new SceneHierarchyObject(parent, @object.gameObject, indent);
				SCENE_HIERARCHY.Add(current);
				if (@object.childCount > 0) FindSceneObject(indent+1, @object, current);
			}
		}

		private static void SelectObject(GameObject gameObject)
		{
			selectObject = gameObject;
				
			OBJECT_COMPONENTS.Clear();
			foreach (Component component in selectObject.GetComponents<Component>())
				OBJECT_COMPONENTS.Add(new ObjectComponent(component));
		}
		
		private static bool CheckUnfolded(SceneHierarchyObject @object)
		{
			bool isUnfolded = true;

			SceneHierarchyObject current = @object;
			while (current.Parent != null)
			{
				if (!current.Parent.IsUnfolded)
				{
					isUnfolded = false;
					break;
				}

				current = current.Parent;
			}

			return isUnfolded;
		}

		private static void FilterObjects()
		{
			if (string.IsNullOrWhiteSpace(searchFilter)) return;
			
			List<SceneHierarchyObject> objects = SCENE_HIERARCHY.Where(obj => ApplyFilter(obj.GameObject.GetFullName())).ToList();
			SCENE_HIERARCHY.Clear();
			SCENE_HIERARCHY.AddRange(objects);
		}

		private static bool ApplyFilter(string toApply)
		{
			string[] filters = searchFilter.ToLowerInvariant().Split('/');
			string option = string.Empty;
			int count = toApply.ToLowerInvariant().Split('/').Length;

			if (count > filters.Length) return toApply.ToLowerInvariant().StartsWith(searchFilter.ToLowerInvariant());
			
			for (int l = 0; l < count; l++)
				option += filters[l] + "/";
			option = option.TrimEnd('/');

			return toApply.ToLowerInvariant().StartsWith(option);
		}
	}
}