using System;
using Simulation.Data;

namespace Simulation.Construction
{
	/// <summary>
	/// The data provider interface provides access to synchronous data access (read-only) to construction related data
	/// </summary>
	public interface IConstructionDataProvider
	{
		event Action<EntityId> ConstructionStartedEvent;
		event Action<EntityId> ConstructionFinishedEvent;
		event Action<EntityId> ProductionFinishedEvent;

		int GetProgress(EntityId buildingViewEntityId);
		bool TryGetProgress(EntityId entityId, out int progress);
		bool IsUnderConstruction(EntityId entityId);
	}
	/// <summary>
	/// the service interface provides write access to construction related data. The access does not happen directly.
	/// Instead, a request is created which will eventually mutate the state at a later point in time.
	/// 
	/// TODO: Implement Lockstep simulation
	/// The data mutation will for now happen instantly.
	/// This will change later when we have our lockstep simulation in place and send the commands over the network
	/// </summary>
	public interface IConstructionService : IConstructionDataProvider
	{
		void RequestStartConstruction(EntityType entityType, SimulationVector3 position, short rotation);
		void RequestStartProduction(EntityType entityType, EntityId factoryEntityId);

		void AddConstructionPoints(EntityId entityId, ushort constructionPointsAmount);
		void CreateFactory(EntityId buildingViewEntityId);
	}

	internal interface IConstructionSimulationService
	{
		void StartConstruction(EntityId entityId, EntityType entityType, SimulationVector3 location, short rotation);
	}
}