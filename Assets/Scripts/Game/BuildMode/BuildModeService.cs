using System;
using Game.AssetLoading;
using Game.Entity;
using Game.InputHandling;
using Game.Utility;
using Simulation.Construction;
using Simulation.Data;
using Simulation.EntityTypeMapping;
using Simulation.Location;
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

	public class BuildModeService : IBuildModeService, ILateTickable, IDisposable
	{
		// use bit-shifting to set the layer mask to layer with ID 6 only, which is our Terrain layer
		public const int TerrainLayerId = 6;

		public event Func<EntityType, bool> OnPlacementConfirmedEvent;

		[Inject] private ICameraRaycastHandler _cameraRaycastHandler;
		[Inject] private IPrefabFactory _prefabFactory;
		[Inject] private IAssetLoadService _assetLoadService;
		[Inject] private IEntityTypeDataProvider _entityTypeDataProvider;
		[Inject] private ILocationDataProvider _locationDataProvider;

		private bool _isBuildModeActive;
		private BuildingView _buildablePreview;
		private EntityType _buildingType;
		private SimulationVector3 _lastValidPosition;

		private readonly IInputHandler _inputHandler;
		private readonly IConstructionService _constructionService;

		public bool IsBuildModeActive => _isBuildModeActive;

		public BuildModeService(IInputHandler inputHandler, IConstructionService constructionService)
		{
			_constructionService = constructionService;
			_inputHandler = inputHandler;

			_inputHandler.OnLeftClickPerformed += ConfirmPlacement;
			_inputHandler.OnRightClickPerformed += CancelBuildMode;
			
			_constructionService.ConstructionStartedEvent += OnConstructionStarted;
		}

		public void LateTick()
		{
			// TODO: Remove this later when we have a BuildMenu in place
			if (Input.GetKeyDown(KeyCode.B)) BuildObjectOfType(EntityType.BuildingC);

			if(!_isBuildModeActive) return;
			if (!TryGetTerrainHitPosition(out var position)) return;
			_lastValidPosition = new SimulationVector3(position.x, position.y, position.z);
			_buildablePreview.SetPosition(_lastValidPosition);
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

			// TODO: create ghost preview of the building at location
			_prefabFactory.ReleaseGameObject(_buildablePreview.gameObject);

			// TODO: Command construction unit to location and start construction task
			_constructionService.RequestStartConstruction(_buildingType, _lastValidPosition, 0);

			Reset();
		}
		
		private void OnConstructionStarted(EntityId constructedEntityId)
		{
			var entityView = _entityTypeDataProvider.Get(constructedEntityId);
			_assetLoadService.LoadAsset<GameObject>(AssetType.GameObject, entityView, OnLoaded);
			
			void OnLoaded(GameObject gameObject)
			{
				var buildingObject = _prefabFactory.CreateGameObject(gameObject);
				var buildingView = buildingObject.GetComponent<BuildingView>();
				buildingView.EntityId = constructedEntityId;
				buildingView.SetPosition(_locationDataProvider.GetLocation(constructedEntityId));
				buildingView.ConfirmPlacement();
			}
		}

		private void Reset()
		{
			_isBuildModeActive = false;
			_buildablePreview = null;
			_buildingType = EntityType.None;
			_lastValidPosition = SimulationVector3.Zero;
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

		public void Dispose()
		{
			_inputHandler.OnLeftClickPerformed -= ConfirmPlacement;
			_inputHandler.OnRightClickPerformed -= CancelBuildMode;

			_constructionService.ConstructionStartedEvent -= OnConstructionStarted;
		}
	}
}
