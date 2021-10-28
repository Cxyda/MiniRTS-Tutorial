using UnityEngine;
using Zenject;

namespace Game.Selection
{
	/// <summary>
	/// This class can be added to GameObjects that can be selected by the player within the game
	/// </summary>
	public class SelectableComponent : MonoBehaviour, ISelectable
	{
		[Inject] private ISelectionService _selectionService;

		// Will be assigned in the Unity Inspector
		[SerializeField] private GameObject SelectionCircleGO;
		[SerializeField] private bool _isSelected;
		private bool _isVisible;

		public bool IsSelected => _isSelected;

		public void Select(bool select)
		{
			_isSelected = select;
			SelectionCircleGO.SetActive(_isSelected);
		}
		private void OnBecameVisible()
		{
			_isVisible = true;
			_selectionService.RegisterSelectable(this);
		}
		private void OnBecameInvisible()
		{
			_isVisible = false;
			// If the object is selected, don't unregister it, otherwise the SelectionService would lose it.
			if (!_isSelected)
			{
				_selectionService.UnregisterSelectable(this);
			}
		}
	}
}