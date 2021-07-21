using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Eden.Core.Utils;
using EdenUnity.Core.Utils;
using HarmonyLib;
using UnityEngine;
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Guu.DevTools.DevMenu
{
	internal class ObjectInspector
	{
		//+ CONSTANTS
		private const int LABEL_WIDTH = 200;
		
		//+ VARIABLES
		private static readonly Dictionary<Type, MethodInfo> DRAW_FUNCTIONS = new Dictionary<Type, MethodInfo>
		{
			{ typeof(Vector3), TypeUtils.GetMethodBySearch(typeof(ObjectInspector), nameof(Vector3Field)) },
			{ typeof(bool), TypeUtils.GetMethodBySearch(typeof(ObjectInspector), nameof(BoolField)) }
		};

		private static readonly HashSet<string> TO_IGNORE = new HashSet<string>()
		{
			"transform",
			"gameObject",
			"tag",
			"name",
			"hideFlags"
		};

		private readonly Dictionary<string, bool> arrayFolds = new Dictionary<string, bool>();
		private readonly Component component;
		private int lastIndent;
		
		//+ CONSTRUCTOR
		internal ObjectInspector(Component component) => this.component = component;

		//+ DRAWING
		// Draws the inspector
		internal void DrawInspector()
		{
			if (component is Transform transform)
				DrawTransform(transform);
			else
			{
				try
				{
					DrawComponent();
				}
				catch (Exception e)
				{
					GUILayout.Label(e.Message + "\n" + e.StackTrace);
				}
			}
		}

		// Draws a transform component
		internal void DrawTransform(Transform transform)
		{
			Vector3Field(transform.localPosition, "Position", false);
			Vector3Field(transform.localEulerAngles, "Rotation", false);
			Vector3Field(transform.localScale, "Scale", false);
		}

		// Draws a unidentified component
		internal void DrawComponent(object @object = null, string extraFieldName = null)
		{
			object current = @object ?? component;
			
			// All Fields
			foreach (FieldInfo field in current.GetType().GetFields(AccessTools.all))
			{
				if (TO_IGNORE.Contains(field.Name) || field.GetCustomAttribute<HideInInspector>() != null) continue;
				if (field.IsStatic) continue;

				bool isPrivate = !field.IsPublic;
				if (isPrivate && field.GetCustomAttribute<SerializeField>() == null) continue;

				string name = isPrivate ? $"[F] <i>{field.Name}</i>" : $"[F] {field.Name}";
				object value = field.GetValue(current);

				if (value != null)
					DrawField(field.FieldType, value, name, isPrivate, $"{(string.IsNullOrWhiteSpace(extraFieldName) ? extraFieldName + "." : string.Empty)}{field.Name}");
				else
					Field(string.Empty, name, isPrivate);
			}
			
			// All Properties
			foreach (PropertyInfo field in current.GetType().GetProperties(AccessTools.all))
			{
				if (TO_IGNORE.Contains(field.Name)) continue;
				
				if (!field.CanRead || field.GetGetMethod().IsStatic) continue;
				bool isPrivate = field.CanRead && !field.GetGetMethod().IsPublic;

				string name = isPrivate ? $"[P] <i>{field.Name}</i>" : $"[P] {field.Name}";
				object value = field.PropertyType.ToString();
				
				try { value = field.GetValue(current); }
				catch { /* Ignored */ }
				
				if (value != null)
					DrawField(field.PropertyType, value, name, isPrivate, $"{(string.IsNullOrWhiteSpace(extraFieldName) ? extraFieldName + "." : string.Empty)}{field.Name}");
				else
					Field(string.Empty, name, isPrivate);
			}
		}

		// Draws a field based on it's type
		private void DrawField(Type type, object value, string label, bool isPrivate, string fieldName)
		{
			if (!DRAW_FUNCTIONS.ContainsKey(type))
			{
				if (type.IsArray)
				{
					//ArrayField(value, label, isPrivate, fieldName);
					Field("Contains " + ((Array) value).Length + " entries", label, isPrivate);
				}
				else
				{
					if (type.GetCustomAttribute<SerializableAttribute>() != null && !type.IsPrimitive)
						//SerializableTypeField(value, label, isPrivate, fieldName);
						Field(value, label, isPrivate);
					else
						Field(value, label, isPrivate);
				}

				return;
			}
			
			DRAW_FUNCTIONS[type].Invoke(this, new []{ value, label, isPrivate });
		}
		
		//+ HELPERS
		private void Field(object value, string label, bool isPrivate)
		{
			GUILayout.BeginHorizontal();
			DrawFieldLabel(label, isPrivate);
			GUILayout.Label(value?.ToString() ?? string.Empty, GUI.skin.textField);
			GUILayout.EndHorizontal();
		}
		
		private void ArrayField(object value, string label, bool isPrivate, string fieldName)
		{
			if (!arrayFolds.ContainsKey(fieldName))
				arrayFolds.Add(fieldName, false);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(4 + lastIndent);
			if (GUILayout.Button(arrayFolds[fieldName] ? InspectorTab.UNFOLDED : InspectorTab.FOLDED, GUI.skin.textField, GUILayout.ExpandWidth(false))) arrayFolds[fieldName] = !arrayFolds[fieldName];
			DrawFieldLabel(label, isPrivate, false);
			GUILayout.EndHorizontal();

			if (!arrayFolds[fieldName]) return;
			
			GUILayout.BeginVertical();

			Array array = (Array) value;
			int i = 0;
			lastIndent += 16;
			foreach (object obj in array)
			{
				DrawField(obj.GetType(), obj, $"Element {i}", false, $"{fieldName}.{i}");
				i++;
			}
			lastIndent -= 16;
			
			GUILayout.EndVertical();
		}
		
		private void SerializableTypeField(object value, string label, bool isPrivate, string fieldName)
		{
			if (!arrayFolds.ContainsKey(fieldName))
				arrayFolds.Add(fieldName, false);
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(4 + lastIndent);
			if (GUILayout.Button(arrayFolds[fieldName] ? InspectorTab.UNFOLDED : InspectorTab.FOLDED, GUI.skin.textField, GUILayout.ExpandWidth(false))) arrayFolds[fieldName] = !arrayFolds[fieldName];
			DrawFieldLabel(label, isPrivate, false);
			GUILayout.EndHorizontal();

			if (!arrayFolds[fieldName]) return;
			
			GUILayout.BeginVertical();

			if (value != null)
			{
				lastIndent += 16;
				try
				{
					DrawComponent(value, fieldName);
				}
				catch (Exception e)
				{
					GUILayout.Label(label + "\n" + e.Message + "\n" + e.StackTrace);
				}

				lastIndent -= 16;
			}
			else
			{
				GUILayout.Label(string.Empty, GUI.skin.textField);
			}

			GUILayout.EndVertical();
		}

		private void BoolField(object value, string label, bool isPrivate)
		{
			GUILayout.BeginHorizontal();
			DrawFieldLabel(label, isPrivate);
			GUILayout.Toggle(Convert.ToBoolean(value), string.Empty);
			GUILayout.EndHorizontal();
		}
		
		private void Vector3Field(object value, string label, bool isPrivate)
		{
			GUILayout.BeginHorizontal();

			DrawFieldLabel(label, isPrivate);

			Vector3 vector = (Vector3) value;
			DrawLabelledField(vector.x, "X");
			GUILayout.Space(5);
			DrawLabelledField(vector.y, "Y");
			GUILayout.Space(5);
			DrawLabelledField(vector.z, "Z");
			
			GUILayout.EndHorizontal();
		}

		private void DrawFieldLabel(string label, bool isPrivate, bool useIndent = true)
		{
			GUI.contentColor = isPrivate ? Color.gray : Color.white;
			GUILayout.Space(4 + (useIndent ? lastIndent : 0));
			GUILayout.Label(label, GUILayout.ExpandWidth(false), GUILayout.Width(LABEL_WIDTH));
			GUI.contentColor = Color.white;
		}
		
		private void DrawLabelledField(object value, string label)
		{
			GUILayout.Label(label, GUILayout.ExpandWidth(false));
			GUILayout.Label(value.ToString(), GUI.skin.textField);
		}
	}
}