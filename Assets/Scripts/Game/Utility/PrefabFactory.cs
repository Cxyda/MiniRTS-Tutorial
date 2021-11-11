using UnityEngine;
using Zenject;

namespace Game.Utility
{
	public interface IPrefabFactory
	{
		GameObject CreateGameObject(GameObject prefab, Vector3 position = default, Quaternion rotation = default,
			Transform parent = null);
		void ReleaseGameObject<T>(T instance) where T : Object;
	}
	/// <summary>
	/// This simple factory class handles the instantiation and destruction of GameObjects.
	/// New objects are instantiated by Zenject, so that Zenject can inject all required dependencies.
	/// </summary>
	public class PrefabFactory : IPrefabFactory
	{
		private readonly DiContainer _container;

		public PrefabFactory(DiContainer container)
		{
			_container = container;
		}

		public GameObject CreateGameObject(GameObject prefab, Vector3 position = default, Quaternion rotation = default,  Transform parent = null)
		{
			// TODO: Add an object pool later on
			var gameObject = _container.InstantiatePrefab(prefab, position, rotation, parent);

			return gameObject;
		}

		public void ReleaseGameObject<T>(T instance) where T : Object
		{
			// TODO: Don't destroy the object each time, use pooling instead
			Object.Destroy(instance);
		}
	}
}