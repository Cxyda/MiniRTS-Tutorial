using System;
using Game.AssetLoading;
using Game.Entity;
using Game.InputHandling;
using Game.Utility;
using Simulation.Data;
using UnityEngine;
using Zenject;

namespace Game.BuildMode
{
	/// <summary>
	/// This class handles the placement of new buildings.
	/// When the build mode is activated it creates a preview version of the building and attaches it to the mouse position
	///
	/// The position of the building is check for validity.
	/// - If the placement is invalid the position cannot be confirmed.
	/// - If the placement is valid publish the <see cref="OnPlacementConfirmedEvent"/> to all subscribers and check whether
	///		they also confirm the placement.
	/// </summary>
	public interface IBuildModeService
	{
		event Func<EntityType, bool> OnPlacementConfirmedEvent;

		void BuildObjectOfType(EntityType entityType);
		void CancelBuildMode();
		bool IsBuildModeActive { get; }
	}

	public class BuildModeService : IBuildModeService, ILateTickable
	{
		// use bit-shifting to set the layer mask to layer with ID 6 only, which is our Terrain layer
		public const int TerrainLayerId = 6;

		public event Func<EntityType, bool> OnPlacementConfirmedEvent;

		private readonly ICameraRaycastHandler _cameraRaycastHandler;
		private readonly IPrefabFactory _prefabFactory;
		private readonly IAssetLoadService _assetLoadService;

		private bool _isBuildModeActive;
		private BuildingView _buildablePreview;
		private int _defaultLayer;
		private EntityType _buildingType;

		public bool IsBuildModeActive => _isBuildModeActive;

		public BuildModeService(ICameraRaycastHandler cameraRaycastHandler, IInputHandler inputHandler,
			IPrefabFactory prefabFactory, IAssetLoadService assetLoadService)
		{
			_cameraRaycastHandler = cameraRaycastHandler;
			_prefabFactory = prefabFactory;
			_assetLoadService = assetLoadService;

			inputHandler.OnLeftClickPerformed += ConfirmPlacement;
			inputHandler.OnRightClickPerformed += CancelBuildMode;
		}

		public void LateTick()
		{
			// TODO: Remove this later when we have a BuildMenu in place
			if (Input.GetKeyDown(KeyCode.B)) BuildObjectOfType(EntityType.BuildingA);

			if(!_isBuildModeActive) return;
			if (!TryGetTerrainHitPosition(out var position)) return;

			_buildablePreview.SetPosition(position);
		}

		public void BuildObjectOfType(EntityType entityType)
		{
			if(_isBuildModeActive) return;

			_assetLoadService.LoadAsset<GameObject>(AssetType.GameObject, entityType, OnLoaded);
			_buildingType = entityType;
			
			void OnLoaded(GameObject gameObject)
			{
				var buildingObject = _prefabFactory.CreateGameObject(gameObject);
				_buildablePreview = buildingObject.GetComponent<BuildingView>();
				_buildablePreview.StartBuildMode();
				_defaultLayer = buildingObject.layer;
				// Layer 2 means Unities "Ignore Raycast" layer to make sure we don't block our own ray-casting
				SetLayerRecursively(buildingObject, 2);
				_isBuildModeActive = true;
			}
		}

		public void CancelBuildMode()
		{
			if (_buildablePreview != null) _prefabFactory.ReleaseGameObject(_buildablePreview.gameObject);
			Reset();
		}

		private void ConfirmPlacement()
		{
			if (!_isBuildModeActive || !_buildablePreview.IsPositionValid) return;

			// Publish the event and check if any subscriber invalidate the construction
			if (!DoAllSubscribersConfirm()) return;
			_buildablePreview.ConfirmPlacement();

			SetLayerRecursively(_buildablePreview.gameObject, _defaultLayer);
			Reset();
		}
		private void Reset()
		{
			_isBuildModeActive = false;
			_buildablePreview = null;
			_buildingType = EntityType.None;
		}

		private bool DoAllSubscribersConfirm()
		{
			// return true when there are no subscribers
			if (OnPlacementConfirmedEvent == null) return true;
			var invocationList = OnPlacementConfirmedEvent.GetInvocationList();
			// Use the '@' character to be able to use reserved key words like 'delegate' as variable names
			foreach (var @delegate in invocationList)
			{
				// cast the delegate to Func<EntityType, bool>
				var func = (Func<EntityType, bool>) @delegate;
				var placementConfirmed = func.Invoke(_buildingType);
				// return false when at least one subscriber returns false
				if (!placementConfirmed) return false;
			}

			return true;
		}

		private bool TryGetTerrainHitPosition(out Vector3 position)
		{
			position = default;
			// Check if we hit any collider
			if (!_cameraRaycastHandler.TryGetHit(out var hit)) return false;
			// Check if the collider is on the Terrain layer
			if (hit.transform.gameObject.layer != TerrainLayerId) return false;
			position = hit.point;
			return true;
		}
		
		/// <summary>
		/// This method assigns all child GameObjects a new layer recursively
		/// </summary>
		private static void SetLayerRecursively(GameObject gameObject, int newLayer)
		{
			if (null == gameObject) return;
           
			gameObject.layer = newLayer;
			foreach (Transform child in gameObject.transform)
			{
				if (null == child) continue;
				SetLayerRecursively(child.gameObject, newLayer);
			}
		}
	}
}
