using Game.BuildMode;
using Simulation.Data;
using UnityEngine;

namespace Game.Entity
{
	/// <summary>
	/// View class for all entities that can be build
	/// </summary>
	public class BuildingView : EntityView
	{
		[SerializeField] protected GameObject model;

		public enum BuildingState
		{
			Preview = 0,
			UnderConstruction = 1,
			Ready = 2
		}

		public BuildingState State => _state;
		public bool IsPositionValid => _buildModeView.IsPlacementValid();

		[SerializeField] private BuildingState _state;
		
		[SerializeField] private BuildModeView _buildModeView;
		[SerializeField] private ConstructionSiteView _constructionSiteView;
		
		private SimulationVector3 _lastPosition;

		public void StartBuildMode()
		{
			_state = BuildingState.Preview;
			_buildModeView.ShowPreview();
		}

		public void SetPosition(SimulationVector3 position)
		{
			if (position == _lastPosition) return;

			transform.position = position.Vector3;
			
			_lastPosition = position;
			_buildModeView.CheckPlacement();
		}

		public void ConfirmPlacement()
		{
			model.SetActive(false);
			_state = BuildingState.UnderConstruction;
			_buildModeView.ConfirmPlacement();

			if (_constructionSiteView != null) _constructionSiteView.StartConstruction();
			else ConstructionFinished();
		}

		public void ConstructionFinished()
		{
			_state = BuildingState.Ready;
			model.SetActive(true);
		}
	}
}