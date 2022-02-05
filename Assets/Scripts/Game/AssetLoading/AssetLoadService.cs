using System;
using Simulation.Data;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.AssetLoading
{
	public interface IAssetLoadService
	{
		void LoadAsset<T>(AssetType assetType, EntityType entityType, Action<T> onLoadedCallback);
	}
	/// <summary>
	/// This class handles finding and loading game assets asynchronously.
	/// It uses Unity's <see cref="Addressables"/> for this.
	/// It identifies the assets via <see cref="AssetType"/> and <see cref="EntityType"/> enums.
	///
	/// When the assets have been loaded it invokes the onLoadedCallback callback.
	/// </summary>
	public class AssetLoadService : IAssetLoadService
	{
		public void LoadAsset<T>(AssetType assetType, EntityType entityType, Action<T> onLoadedCallback)
		{
			// Load the asset with the given address (key) and call OnLoaded when it's done
			Addressables.LoadAssetAsync<T>($"{assetType.ToString()}/{entityType.ToString()}").Completed += OnLoaded;

			void OnLoaded(AsyncOperationHandle<T> handle)
			{
				// Invoke the callback and pass the asset back
				onLoadedCallback.Invoke(handle.Result);
			}
		}
	}
}