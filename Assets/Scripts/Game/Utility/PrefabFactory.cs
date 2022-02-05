using System;
using Game.AssetLoading;
using Simulation.Data;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Utility
{
	public interface IPrefabFactory
	{
		void CreateGameObject(AssetType assetType, EntityType entityType, Action<GameObject> onCreatedCallback,
			Vector3 position = default, Quaternion rotation = default, Transform parent = null);
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
		[Inject] private IAssetLoadService _assetLoadService;

		private readonly DiContainer _container;

		public PrefabFactory(DiContainer container)
		{
			_container = container;
		}

		public void CreateGameObject(AssetType assetType, EntityType entityType, Action<GameObject> onCreatedCallback,
			Vector3 position = default, Quaternion rotation = default, Transform parent = null)
		{
			_assetLoadService.LoadAsset<GameObject>(assetType, entityType, OnLoaded);
			
			void OnLoaded(GameObject obj)
			{
				var gameObject = CreateGameObject(obj, position, rotation, parent);
				onCreatedCallback(gameObject);
			}
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