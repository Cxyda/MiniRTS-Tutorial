using Simulation.Data;

namespace Simulation.EntityTypeMapping
{
    /// <summary>
    /// This struct stores the type of an entity to be able to get a <see cref="EntityId"/> to <see cref="EntityId"/> mapping
    /// </summary>
    internal struct EntityTypeVO
    {
        internal EntityId EntityId;
        internal EntityType EntityType;

        internal EntityTypeVO(EntityId entityId, EntityType entityType)
        {
            EntityId = entityId;
            EntityType = entityType;
        }
    }
}
