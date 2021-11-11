using Game.BuildMode;
using UnityEngine;

namespace Game.Entity
{
	/// <summary>
	/// This view component needs to be attached to all buildable objects
	/// it serves as an interface between the player and the game services to control the state
	/// of the GameWorld object
	/// </summary>
	public class BuildingView : MonoBehaviour
	{
		[SerializeField] protected GameObject model;
		[SerializeField] private BuildModeView _buildModeView;

		public bool IsPositionValid => _buildModeView.IsPlacementValid();

		public void SetPosition(Vector3 position)
		{
			transform.position = position;
			_buildModeView.CheckPlacement();
		}

		public void ConfirmPlacement()
		{
			model.SetActive(false);
			_buildModeView.ConfirmPlacement();

			ConstructionFinished();
		}

		public void ConstructionFinished()
		{
			model.SetActive(true);
		}

		public void StartBuildMode()
		{
			_buildModeView.ShowPreview();
		}
	}
}