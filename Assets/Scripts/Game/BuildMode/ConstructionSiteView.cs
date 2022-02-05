using Game.Entity;
using Game.Utility;
using Simulation.Construction;
using Simulation.Data;
using Simulation.Simulation;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Game.BuildMode
{
	/// <summary>
	/// This view component handles the construction site of a buildable object
	/// </summary>
	[RequireComponent(typeof(BuildingView))]
	public class ConstructionSiteView : MonoBehaviour
	{
		[Inject] private IConstructionService _constructionService;
		[Inject] private ISimulationService _simulationService;
		[Inject] private IPrefabFactory _prefabFactory;

		[SerializeField] private BuildingView _buildingView;
		[SerializeField] private ConstructionSiteViewData[] _constructionSites;

		private GameObject _currentConstructionSiteObject;
		private int _lastConstructionSiteIndex = -1;

		private void Awake()
		{
			// If the building is pre-placed, cleanup the construction
			if(_buildingView.State == BuildingView.BuildingState.Ready) CleanupConstruction();
		}

		public void StartConstruction()
		{
			enabled = true;
			
			// TODO: Remove this after we implemented construction workers
			_simulationService.SimulationTickEvent += OnSimulationTick;
			_constructionService.ConstructionFinishedEvent += OnConstructionFinished;
		}

		private void UpdateConstructionSiteView()
		{
			if (!_constructionService.TryGetProgress(_buildingView.EntityId, out var constructionProgress))
			{
				return;
			}

			for (var i = _constructionSites.Length - 1; i >= 0; i--)
			{
				var viewData = _constructionSites[i];
				if (viewData.Progress >= constructionProgress) continue;
				if (_lastConstructionSiteIndex == i) break;

				var objectTransform = transform;
				viewData.Asset.InstantiateAsync(objectTransform.position, objectTransform.rotation, objectTransform.parent).Completed += OnInstantiated;
				void OnInstantiated(AsyncOperationHandle<GameObject> result)
				{
					Destroy(_currentConstructionSiteObject);
					_currentConstructionSiteObject = result.Result;
					_lastConstructionSiteIndex = i;
				}
				break;
			}
		}

		private void OnConstructionFinished(EntityId entityId)
		{
			if (entityId != _buildingView.EntityId)
				return;

			FinishConstruction();
		}
		private void OnSimulationTick()
		{
			_constructionService.AddConstructionPoints(_buildingView.EntityId, 1);
			UpdateConstructionSiteView();
		}

		private void FinishConstruction()
		{
			_simulationService.SimulationTickEvent -= OnSimulationTick;
			_constructionService.ConstructionFinishedEvent -= OnConstructionFinished;

			_buildingView.ConstructionFinished();
			CleanupConstruction();
		}

		private void CleanupConstruction()
		{
			Destroy(this);
			Destroy(_currentConstructionSiteObject);
		}
	}
}