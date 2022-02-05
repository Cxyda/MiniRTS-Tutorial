using System.Collections.Generic;
using Game.Selection;
using Simulation.Data;
using UnityEngine;
using Zenject;

namespace Game.UiViews.Hud
{
	/// <summary>
	/// For this tutorial section we quick and dirty implement a basic UI. This will be refactored in the next sections.
	/// </summary>
	public class HudPresenter : MonoBehaviour
	{
		[Inject]
		private ISelectionService _selectionService;

		private void Awake()
		{
			_selectionService.SelectionChangedEvent += OnSelectionChanged;
		}
		private void OnDestroy()
		{
			_selectionService.SelectionChangedEvent -= OnSelectionChanged;
		}

		private void OnSelectionChanged(IReadOnlyList<EntityId> selectedEntityIds)
		{
			// TODO: Just show the factory HUD for now if there are any entities selected
			if (selectedEntityIds.Count > 0)
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}
