using System;
using Game.Selection;
using UnityEngine;
using Zenject;

namespace Game.InputHandling
{
	/// <summary>
	/// This class casts rays from the camera through the mouse position to check whether we clicked on an object
	/// with a <see cref="SelectableComponent"/> attached.
	/// </summary>
	public interface IRaycastHandler
	{
		event Action<SelectableComponent, bool, bool> OnSelectionPerformedEvent;
		bool TryGetHit(out RaycastHit hit, float distance = 1000f, LayerMask layerMask = default);
	}

	public class RaycastHandler : IRaycastHandler, IInitializable
	{
		/// <summary>
		/// Event which gets invoked when a selectable entity has been clicked
		/// First parameter is the selectable component that has received the click
		/// Second parameter is whether the selectable should be added to the previous selection or if it should be replaced
		/// Third parameter is whether a double click has been performed on that selectable or not
		/// </summary>
		public event Action<SelectableComponent, bool, bool> OnSelectionPerformedEvent;

		[Inject] private InputHandler _inputHandler;

		private const int IgnoreRaycastLayer = 2;
		private Camera _camera;

		public void Initialize()
		{
			_inputHandler.OnLeftClickPerformed += OnLeftClickPerformed;
			_inputHandler.OnDoubleLeftClickPerformed += OnDoubleLeftClickPerformed;
			_camera = Camera.main;
		}
		private void OnLeftClickPerformed()
		{
			if (!TryGetHit(out var hit))
			{
				return;
			}

			if (TryGetSelectable(hit, out var selectable))
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
			if (!TryGetHit(out var hit))
			{
				return;
			}

			if (TryGetSelectable(hit, out var selectable))
			{
				OnSelectionPerformedEvent?.Invoke(selectable, _inputHandler.ModifySelection, true);
			}
		}

		public bool TryGetHit(out RaycastHit hit, float distance = 1000f, LayerMask layerMask = default)
		{
			hit = default;
			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			if (layerMask == default)
			{
				// default means bitmask of 0, so to raycast against everything except the "Ignore Raycast" layer inverse the bitmask.
				layerMask = -IgnoreRaycastLayer;
			}
			return Physics.Raycast(ray, out hit, distance);
		}

		private bool TryGetSelectable(RaycastHit hit, out SelectableComponent selectable)
		{
			selectable = null;

			// We did hit a Collider. Check if the gameObject has a SelectableComponent attached
			selectable = hit.transform.GetComponent<SelectableComponent>();
			return selectable != null;
		}
	}
}