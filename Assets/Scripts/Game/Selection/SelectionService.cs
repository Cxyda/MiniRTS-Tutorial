// ReSharper disable ClassNeverInstantiated.Global

using System.Collections.Generic;
using Game.InputHandling;
using UnityEngine;

namespace Game.Selection
{
	/// <summary>
	/// This class handles the selection of <see cref="SelectableComponent"/> entities.
	/// </summary>
	public interface ISelectionService
	{
		void Select(SelectableComponent selectable, bool addToSelection = false, bool selectAllOfSameType = false);

		void RegisterSelectable(SelectableComponent selectableComponent);
		void UnregisterSelectable(SelectableComponent selectableComponent);
	}

	public class SelectionService : ISelectionService
	{
		private readonly HashSet<SelectableComponent> _selectedEntities;
		private readonly HashSet<SelectableComponent> _visibleSelectables;
		private readonly InputHandler _inputHandler;
		private readonly Camera _camera;

		private readonly HashSet<SelectableComponent>[] _selectionGroups;

		public SelectionService(CameraRaycastHandler raycastHandler, InputHandler inputHandler)
		{
			// Initialize our HashSets
			_selectedEntities = new HashSet<SelectableComponent>();
			_visibleSelectables = new HashSet<SelectableComponent>();

			_selectionGroups = new HashSet<SelectableComponent>[10];
			_inputHandler = inputHandler;

			_inputHandler.OnSelectionRectChanged += GetEntitiesWithinSelectionRect;
			_inputHandler.OnSelectionGroupSaved += SaveSelection;
			_inputHandler.OnSelectionGroupRestored += RestoreSelection;
			raycastHandler.OnSelectionPerformedEvent += Select;

			_camera = Camera.main;
		}
		public void Select(SelectableComponent selectable, bool addToSelection = false, bool selectAllOfSameType = false)
		{
			if (!addToSelection)
			{
				ClearSelection();
			}

			if (selectable == null) return;

			// Handle the special case when we want to modify a selection and click on an already selected object.
			if (addToSelection && _selectedEntities.Contains(selectable))
			{
				selectable.Select(false);
				_selectedEntities.Remove(selectable);
			}
			else
			{
				selectable.Select(true);
				_selectedEntities.Add(selectable);
			}
			//TODO: Add functionality to select all objects of the same type on screen
		}
		private void GetEntitiesWithinSelectionRect(Rect? selectionRect)
		{
			if (!_inputHandler.ModifySelection)
			{
				ClearSelection();
			}
			// If the rect is null, just return
			if (!selectionRect.HasValue)
			{
				return;
			}
			// check all objects stored in the _visibleSelectables if their positions are within the selection rect
			foreach (var selectable in _visibleSelectables)
			{
				var screenPoint = GetScreenPoint(selectable);
				if (selectionRect.Value.Contains(screenPoint))
				{
					Select(selectable, true, false);
				}
			}
		}

		private Vector3 GetScreenPoint(SelectableComponent selectable)
		{
			var screenPoint = _camera.WorldToScreenPoint(selectable.transform.position);
			// Move origin from bottom left to top left
			screenPoint.y = Screen.height - screenPoint.y;
			// reset z-coordinate just to be sure
			screenPoint.z = 0;
			return screenPoint;
		}

		public void RegisterSelectable(SelectableComponent selectableComponent)
		{
			_visibleSelectables.Add(selectableComponent);
		}

		public void UnregisterSelectable(SelectableComponent selectableComponent)
		{
			_visibleSelectables.Remove(selectableComponent);
		}
		private void SaveSelection(byte selectionGroupIndex)
		{
			if(selectionGroupIndex >= _selectionGroups.Length) return;
			// Get the selection HashSet or create a new one if it's null
			var selection = _selectionGroups[selectionGroupIndex] ?? new HashSet<SelectableComponent>();

			selection.Clear();
			// Iterate over _selectedEntities and add the entities one by one instead of assigning it directly,
			// otherwise we would assign the reference to the HashSet instead of its values
			foreach (var selectedEntity in _selectedEntities)
			{
				selection.Add(selectedEntity);
			}
			_selectionGroups[selectionGroupIndex] = selection;
		}
		private void RestoreSelection(byte selectionGroupIndex)
		{
			if(selectionGroupIndex >= _selectionGroups.Length) return;
			var selection = _selectionGroups[selectionGroupIndex];
			if(selection == null) return;
			if(!_inputHandler.ModifySelection) ClearSelection();

			foreach (var selectedEntity in selection)
			{
				Select(selectedEntity, true);
			}
		}
		private void ClearSelection()
		{
			foreach (var selectedEntity in _selectedEntities)
			{
				selectedEntity.Select(false);
			}
			_selectedEntities.Clear();
		}

	}
}