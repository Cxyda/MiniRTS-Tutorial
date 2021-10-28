using System;
using Game.InputHandling;
using UnityEngine;
using Zenject;

namespace Game.Selection
{
	/// <summary>
	/// This class casts rays from the camera through the mouse position to check whether we clicked on an object
	/// with a <see cref="SelectableComponent"/> attached.
	/// </summary>
	public class SelectableRaycastComponent : IInitializable
	{
		/// <summary>
		/// Event which gets invoked when a selectable entity has been clicked
		/// First parameter is the selectable component that has received the click
		/// Second parameter is whether the selectable should be added to the previous selection or if it should be replaced
		/// Third parameter is whether a double click has been performed on that selectable or not
		/// </summary>
		public event Action<SelectableComponent, bool, bool> OnSelectionPerformedEvent;
		
		private Camera _camera;
		[Inject] private InputHandler _inputHandler;
		
		public void Initialize()
		{
			_inputHandler.OnLeftClickPerformed += OnLeftClickPerformed;
			_inputHandler.OnDoubleLeftClickPerformed += OnDoubleLeftClickPerformed;
			_camera = Camera.main;
		}
		private void OnLeftClickPerformed()
		{
			if (TryGetSelectable(out var selectable))
			{
				// Invoke the event passing the selectable, whether the user presses the ModifySelection key and whether we want
				// to select all all objects of the same type as well
				OnSelectionPerformedEvent?.Invoke(selectable, _inputHandler.ModifySelection, false);
			}
			else
			{
				// In this case we didn't click on a selectable object so pass null and clear the selection
				OnSelectionPerformedEvent?.Invoke(null, false, false);
			}
		}
		private void OnDoubleLeftClickPerformed()
		{
			if (TryGetSelectable(out var selectable))
			{
				OnSelectionPerformedEvent?.Invoke(selectable, _inputHandler.ModifySelection, true);
			}
		}
		private bool TryGetSelectable(out SelectableComponent selectable)
		{
			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			selectable = null;
			if (!Physics.Raycast(ray, out var hit))
			{
				return false;
			}
			// We did hit a Collider. Check if the gameObject has a SelectableComponent attached
			selectable = hit.transform.GetComponent<SelectableComponent>();
			return selectable != null;
		}
	}
}