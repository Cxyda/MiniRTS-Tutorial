using System;
using System.Collections.Generic;
using System.Linq;
using Game.Selection;
using Simulation.Construction;
using Simulation.Data;
using UnityEngine;
using Zenject;

namespace Game.Factory
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public interface IFactoryService
	{
		void ProduceObjectOfType(EntityType entityType);
	}
	public class FactoryService : IFactoryService, IDisposable, ILateTickable
	{
		private IReadOnlyList<EntityId> _selectedEntityIds;

		private readonly ISelectionService _selectionService;
		private readonly IConstructionService _constructionService;

		private readonly IList<EntityId> _tempFilteredList;

		public FactoryService(ISelectionService selectionService, IConstructionService constructionService)
		{
			_selectionService = selectionService;
			_constructionService = constructionService;
			selectionService.SelectionChangedEvent += OnSelectionChanged;

			_tempFilteredList = new List<EntityId>();
		}

		public void LateTick()
		{
			// TODO: Remove this later when we have a FactoryUI in place
			if (Input.GetKeyDown(KeyCode.U)) ProduceObjectOfType(EntityType.UnitA);
		}

		public void Dispose()
		{
			_selectionService.SelectionChangedEvent -= OnSelectionChanged;
		}

		public void ProduceObjectOfType(EntityType entityType)
		{
			if (!IsProductionValidForCurrentSelection(entityType))
			{
				return;
			}
			var filteredList = FilterSelectionForFactoriesWithProductOf(entityType);
			foreach (var factoryId in filteredList)
			{
				_constructionService.RequestStartProduction(entityType, factoryId);
			}
		}
		private IEnumerable<EntityId> FilterSelectionForFactoriesWithProductOf(EntityType entityType)
		{
			_tempFilteredList.Clear();
			_tempFilteredList.Add(_selectedEntityIds[0]);
			return _tempFilteredList;
		}

		private bool IsProductionValidForCurrentSelection(EntityType entityType)
		{
			// TODO: check if that entityType can be produced by any of the selected factories
			return _selectedEntityIds.Count > 0;
		}
		private void OnSelectionChanged(IReadOnlyList<EntityId> selectedEntityIds)
		{
			_selectedEntityIds = selectedEntityIds;
		}


	}

}
