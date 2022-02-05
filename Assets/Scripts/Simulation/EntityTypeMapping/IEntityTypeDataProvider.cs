using System.Collections.Generic;
using Simulation.Data;

namespace Simulation.EntityTypeMapping
{
    /// <summary>
    /// TODO: 
    /// </summary>
    public interface IEntityTypeDataProvider
    {
        EntityType Get(EntityId entityId);
    }
    public interface IEntityTypeSimulationService : IEntityTypeDataProvider
    {
        void Add(EntityId entityId, EntityType entityType);
        void Remove(EntityId entityId);
    }
}
