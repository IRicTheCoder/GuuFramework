using System.Collections.Generic;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace Guu
{
	/// <summary>
	/// A class that can access all objects within the game
	/// </summary>
	public static class SRObjects
	{
		/// <summary>The missing icon sprite</summary>
		public static Sprite MissingIcon { get; }

		/// <summary>The missing image sprite</summary>
		public static Sprite MissingImg { get; }

		// Static Constructor that helps load base objects on first use
		static SRObjects()
		{
			MissingIcon = Get<Sprite>("unknownSmall");
			MissingImg = Get<Sprite>("unknownLarge");
		}
		
		/// <summary>
		/// Gets an object of a type by its name
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static T Get<T>(string name) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="type">Type to search</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static Object Get(string name, System.Type type)
		{
			foreach (Object found in Resources.FindObjectsOfTypeAll(type))
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets all objects of a type
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		public static List<T> GetAll<T>() where T : Object
		{
			return new List<T>(Resources.FindObjectsOfTypeAll<T>());
		}

		/// <summary>
		/// Gets an instance of an object of a type by its name
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <returns>Instance of object found or null if nothing is found</returns>
		public static T GetInst<T>(string name) where T : Object
		{
			foreach (T found in Resources.FindObjectsOfTypeAll<T>())
			{
				if (found.name.Equals(name))
					return Object.Instantiate(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an instance of an object of a type by its name
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="type">Type to search</param>
		/// <returns>Instance of object found or null if nothing is found</returns>
		public static Object GetInst(string name, System.Type type)
		{
			foreach (Object found in Resources.FindObjectsOfTypeAll(type))
			{
				if (found.name.Equals(name))
					return Object.Instantiate(found);
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name in the world
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		/// <param name="name">Name to search for</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static T GetWorld<T>(string name) where T : Object
		{
			foreach (T found in Object.FindObjectsOfType<T>())
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets an object of a type by its name in the world
		/// </summary>
		/// <param name="name">Name to search for</param>
		/// <param name="type">Type to search</param>
		/// <returns>Object found or null if nothing is found</returns>
		public static Object GetWorld(string name, System.Type type)
		{
			foreach (Object found in Object.FindObjectsOfType(type))
			{
				if (found.name.Equals(name))
					return found;
			}

			return null;
		}

		/// <summary>
		/// Gets all objects of a type in the world
		/// </summary>
		/// <typeparam name="T">Type to search</typeparam>
		public static List<T> GetAllWorld<T>() where T : Object
		{
			return new List<T>(Object.FindObjectsOfType<T>());
		}
	}
}