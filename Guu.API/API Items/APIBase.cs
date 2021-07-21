using JetBrains.Annotations;
using UnityEngine;

namespace Guu.API
{
	/// <summary>
	/// The base of all API items, these are used to register new content to the game
	/// </summary>
	/// <typeparam name="T">The type of API Item</typeparam>
	public abstract class APIBase<T> : IBuilder where T : APIBase<T>
	{
		//+ PROPERTIES
		/// <summary>The name of this object</summary>
		[UsedImplicitly]
		protected string Name { get; set; }

		///<summary>The prefix for the name of this object</summary>
		protected abstract string NamePrefix { get; }

		/// <summary>The fake version to later replace with the real one</summary>
		protected GameObject FakePrefab { get; private set; }

		/// <inheritdoc />
		public abstract GameObject BaseItem { get; }

		//+ METHODS
		/// <inheritdoc />
		public virtual void Build(GameObject prefab) => prefab.name = FakePrefab.name;

		/// <summary>Creates the API object and gets it ready to register</summary>
		[UsedImplicitly]
		public virtual T Create()
		{
			FakePrefab = new GameObject(NamePrefix + Name);
			FakePrefab.transform.parent = APIHandler.PrefabHolder.transform;

			APIUtils.APIPrefab apiPrefab = FakePrefab.AddComponent<APIUtils.APIPrefab>();
			apiPrefab.apiBuilder = this;

			return (T) this;
		}

		/// <summary>Registers the API Item</summary>
		[UsedImplicitly]
		public abstract void Register();
	}
}