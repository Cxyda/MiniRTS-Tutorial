using Simulation.Data;

namespace Simulation.EntityTypeMapping
{
    /// <summary>
    /// TODO: 
    /// </summary>
    public class EntityTypeService : IEntityTypeSimulationService, IEntityTypeDataProvider
    {
        private readonly EntityTypeModel _entityTypeModel;

        internal EntityTypeService(EntityTypeModel entityTypeModel)
        {
            _entityTypeModel = entityTypeModel;
        }

        public void Add(EntityId entityId, EntityType entityType)
        {
            _entityTypeModel.DataResolver.Add(entityId, new EntityTypeVO(entityId, entityType));
        }
        public void Remove(EntityId entityId)
        {
            _entityTypeModel.DataResolver.Remove(entityId);
        }

        public EntityType Get(EntityId entityId)
        {
            return _entityTypeModel.DataResolver.Get(entityId).EntityType;
        }
    }
}
