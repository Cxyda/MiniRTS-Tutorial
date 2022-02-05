using System;
using Simulation.Data;
using Simulation.EntityTypeMapping;
using Simulation.GameTime;
using Simulation.Location;
using Simulation.Simulation;
using UnityEngine;
using Zenject;

namespace Simulation.Construction
{
	/// <summary>
	/// The <see cref="ConstructionService"/> handles construction requests, creates commands and passes them to the
	/// Simulation logic where they get executed. Then, this service gets called again to mutate the game state which is stored
	/// in the <see cref="ConstructionModel"/>. 
	/// </summary>
	public class ConstructionService : IConstructionService, IConstructionSimulationService
	{
		public event Action<EntityId> ConstructionStartedEvent;
		public event Action<EntityId> ConstructionFinishedEvent;
		public event Action<EntityId> ProductionFinishedEvent;

		public event Action<EntityId> ConstructionFailedEvent;

		private readonly ConstructionModel _constructionModel;

		[Inject]
		private IEntityIdFactory _entityIdFactory;
		[Inject]
		private IEntityTypeSimulationService _entityTypeSimulationService;
		[Inject]
		private ISimulationService _simulationService;
		[Inject]
		private ILocationSimulationService _locationSimulationService;
		[Inject]
		private IEntityTypeDataProvider _entityTypeDataProvider;

		internal ConstructionService(ConstructionModel constructionModel)
		{
			_constructionModel = constructionModel;
		}

		/// <summary>
		/// Returns the current construction progress [0,100] in percent
		/// </summary>
		public int GetProgress(EntityId entityId)
		{
			if (!TryGetProgress(entityId, out var progress))
			{
				throw new Exception($"Construction progress for entityId {entityId} could not be found.");
			}
			return progress;
		}

		public bool TryGetProgress(EntityId entityId, out int progress)
		{
			progress = 0;
			if (!_constructionModel.Constructions.TryGetValue(entityId, out var data))
			{
				return false;
			}
			var constructionPointsRequired = GetRequiredConstructionPoints(_entityTypeDataProvider.Get(entityId));

			progress = Mathf.RoundToInt((float)data.ConstructionPointsInvested / constructionPointsRequired * 100);
			return true;
		}
		public bool IsUnderConstruction(EntityId entityId)
		{
			if (!TryGetProgress(entityId, out var progress))
			{
				return false;
			}
			return progress >= 100;
		}
		/// <summary>
		/// Called by the <see cref="BuildModeService"/> when the player has built something
		/// </summary>
		public void RequestStartConstruction(EntityType entityType, SimulationVector3 position, short rotation)
		{
			// TODO: Get and pass the correct local playerID
			var entityId = _entityIdFactory.CreateId(0);

			// Create all data already, so that the views are able to query the data. If something goes wrong
			// in the simulation step, we will need to clear the data again
			_entityTypeSimulationService.Add(entityId, entityType);
			_constructionModel.Constructions.Add(entityId, new ConstructionVO(entityId));
			_locationSimulationService.SetLocation(entityId, position, rotation);

			// TODO: Don't call this directly after the Simulation code has been implemented
			StartConstruction(entityId, entityType, position, rotation);
			// TODO: Uncomment this when the simulation code has been implemented
			// TODO: var command = new StartConstructionCommand(_timeService.GetGameTime(), entityId, entityType, position, rotation);
			// TODO: _simulationQueueService.QueueCommand(command);

			ConstructionStartedEvent?.Invoke(entityId);
		}

		/// <summary>
		/// Called by the <see cref="FactoryService"/> to produce something in factories
		/// </summary>
		public void RequestStartProduction(EntityType entityType, EntityId factoryEntityId)
		{
			// TODO: Don't call this directly after the Simulation code has been implemented
			StartProduction(entityType, factoryEntityId);
		}

		public void AddConstructionPoints(EntityId entityId, ushort constructionPointsAmount)
		{
			var scaledPointsToAdd = constructionPointsAmount * ConstructionVO.ConstructionPointsScaleFactor;
			var constructionPointsRequired = GetRequiredConstructionPoints(_entityTypeDataProvider.Get(entityId));

			if (_constructionModel.Constructions.Contains(entityId))
			{
				var data = _constructionModel.Constructions.Get(entityId);
				data.ConstructionPointsInvested += (ushort)scaledPointsToAdd;

				_constructionModel.Constructions.Set(entityId, data);
				if (data.ConstructionPointsInvested >= constructionPointsRequired)
				{
					_constructionModel.Constructions.Remove(entityId);
					ConstructionFinishedEvent?.Invoke(entityId);
				}
			}
			else
			{
				var data = _constructionModel.Factories.Get(entityId);
				if (data.EntityId == EntityId.Invalid)
				{
					if (data.ProductionQueue.Count == 0)
					{
						return;
					}
					var productionType = data.ProductionQueue.Dequeue();
					StartNewProduction(ref data, productionType);
				}

				data.ProductionPointsInvested += (ushort)scaledPointsToAdd;

				_constructionModel.Factories.Set(entityId, data);
				if (data.ProductionPointsInvested >= constructionPointsRequired)
				{
					data.ProductionEntityId = EntityId.Invalid;
					ProductionFinishedEvent?.Invoke(entityId);
				}
			}
		}
		public void CreateFactory(EntityId factoryId)
		{
			_constructionModel.Factories.Add(factoryId, new FactoryVO());
		}

		public void StartConstruction(EntityId entityId, EntityType entityType, SimulationVector3 location, short rotation)
		{
			// TODO: pass area of the construction as well to the location service
			if (_locationSimulationService.IsAreaOccupied(location))
			{
				ConstructionFailed(entityId, entityType);
				return;
			}
		}

		private void ConstructionFailed(EntityId entityId, EntityType entityType)
		{
			_entityTypeSimulationService.Remove(entityId);
			_constructionModel.Constructions.Remove(entityId);
			_locationSimulationService.Remove(entityId);
			
			ConstructionFailedEvent?.Invoke(entityId);
		}

		public void StartProduction(EntityType entityType, EntityId factoryId)
		{
			var factoryVo = _constructionModel.Factories.Get(factoryId);

			if (factoryVo.ProductionQueue.Count > 0)
			{
				factoryVo.ProductionQueue.Enqueue(entityType);
			}
			else
			{
				StartNewProduction(ref factoryVo, entityType);
			}
			_constructionModel.Factories.Set(factoryVo.EntityId, factoryVo);
		}
		private void StartNewProduction(ref FactoryVO factoryVo, EntityType entityType)
		{
			// TODO: Get and pass the correct local playerID
			var entityId = _entityIdFactory.CreateId(0);
			_entityTypeSimulationService.Add(entityId, entityType);

			factoryVo.ProductionEntityId = entityId;
			factoryVo.ProductionPointsInvested = 0;
		}

		private int GetRequiredConstructionPoints(EntityType entityType)
		{
			// TODO: Get the required construction points for this construction from the GD config.
			// For now we assume 100 points are required for everything
			return 20 * ConstructionVO.ConstructionPointsScaleFactor * _simulationService.SimulationFrequency;
		}
	}
}