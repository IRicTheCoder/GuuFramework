using System;
using System.Reflection;
using Guu.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
/// <summary>Contains extension methods for Objects</summary>
public static class ObjectExt
{
	//+ SYSTEM OBJECT
	/// <summary>
	/// Sets a value to a private field
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="obj">The object you are setting the value in</param>
	/// <param name="name">The name of the field</param>
	/// <param name="value">The value to set</param>
	public static T SetPrivateField<T>(this T obj, string name, object value)
	{
		return ExceptionUtils.IgnoreErrors(() =>
		{
			FieldInfo field = (obj is Type type ? type : obj.GetType()).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			field?.SetValue(obj, value);
			
			return obj;
		}, obj);
	}

	/// <summary>
	/// Sets a value to a private property
	/// </summary>
	/// <typeparam name="T">Type of object</typeparam>
	/// <param name="obj">The object you are setting the value in</param>
	/// <param name="name">The name of the property</param>
	/// <param name="value">The value to set</param>
	public static T SetPrivateProperty<T>(this T obj, string name, object value)
	{
		return ExceptionUtils.IgnoreErrors(() =>
		{
			PropertyInfo field = (obj is Type type ? type : obj.GetType()).GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);

			if (field == null) return obj;

			if (field.CanWrite)
				field.SetValue(obj, value, null);
			else
				return obj.SetPrivateField($"<{name}>k__BackingField", value);

			return obj;
		}, obj);
	}

	/// <summary>
	/// Gets the value of a private field
	/// </summary>
	/// <typeparam name="E">Type of value</typeparam>
	/// <param name="obj">The object to get the value from</param>
	/// <param name="name">The name of the field</param>
	public static E GetPrivateField<E>(this object obj, string name)
	{
		return ExceptionUtils.IgnoreErrors(() =>
		{
			FieldInfo field = (obj is Type type ? type : obj.GetType()).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			return (E)field?.GetValue(obj);
		});
	}

	/// <summary>
	/// Gets the value of a private property
	/// </summary>
	/// <typeparam name="E">Type of value</typeparam>
	/// <param name="obj">The object to get the value from</param>
	/// <param name="name">The name of the property</param>
	public static E GetPrivateProperty<E>(this object obj, string name)
	{
		return ExceptionUtils.IgnoreErrors(() =>
		{
			PropertyInfo field = (obj is Type type ? type : obj.GetType()).GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
			
			if (field == null) return default;
			
			return field.CanRead ? (E)field.GetValue(obj, null) : obj.GetPrivateField<E>($"<{name}>k__BackingField");
		});
	}

	/// <summary>
	/// Sets the value of an object to the value of a private field on another object
	/// </summary>
	/// <param name="obj">The object to set</param>
	/// <param name="from">The object to get from</param>
	/// <param name="name">The name of the field</param>
	/// <typeparam name="T">The type of object being set</typeparam>
	/// <typeparam name="E">The type of object getting from</typeparam>
	/// <returns>The object itself for convenience</returns>
	// ReSharper disable once RedundantAssignment
	public static T SetFromPrivate<T, E>(this T obj, E from, string name)
	{
		obj = from.GetPrivateField<T>(name);

		return obj;
	}

	//+ UNITY OBJECT
	/// <summary>
	/// Clones the object
	/// </summary>
	public static T CloneInstance<T>(this T obj) where T : Object
	{
		return Object.Instantiate(obj);
	}

	/// <summary>
	/// Is the component present in the object?
	/// </summary>
	/// <param name="obj">Object to test</param>
	/// <param name="comp">The component if found, null if not</param>
	/// <typeparam name="T">The type of component</typeparam>
	/// <returns>True if the component is found, false otherwise</returns>
	public static bool HasComponent<T>(this Component obj, out T comp) where T : Component
	{
		comp = obj.GetComponent<T>();

		return comp != null;
	}
	
	/// <summary>
	/// Is the component present in the object?
	/// </summary>
	/// <param name="obj">Object to test</param>
	/// <typeparam name="T">The type of component</typeparam>
	/// <returns>True if the component is found, false otherwise</returns>
	public static bool HasComponent<T>(this Component obj) where T : Component
	{
		return obj.GetComponent<T>() != null;
	}

	/// <summary>
	/// Is the component present in the object?
	/// </summary>
	/// <param name="obj">Object to test</param>
	/// <param name="comp">The component if found, null if not</param>
	/// <typeparam name="T">The type of component</typeparam>
	/// <returns>True if the component is found, false otherwise</returns>
	public static bool HasComponent<T>(this GameObject obj, out T comp) where T : Component
	{
		comp = obj.GetComponent<T>();

		return comp != null;
	}
	
	/// <summary>
	/// Is the component present in the object?
	/// </summary>
	/// <param name="obj">Object to test</param>
	/// <typeparam name="T">The type of component</typeparam>
	/// <returns>True if the component is found, false otherwise</returns>
	public static bool HasComponent<T>(this GameObject obj) where T : Component
	{
		return obj.GetComponent<T>() != null;
	}
}