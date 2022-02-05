using System;
using Game.Entity;
using Simulation;
using Simulation.Data;

namespace Game.EntityReference
{
    /// <summary>
    /// This service maps entityIDs to game world objects.
    /// As soon as an entity objects gets registered it should register itself to this service with its id.
    /// It then can be found and referenced by its ID
    /// If an entity gets removed / destroyed it needs to unregister itself.
    /// </summary>
    public interface IEntityReferenceService
    {
        EntityView GetEntityViewBy(EntityId entityId);
        void Register(EntityId entityId, EntityView entityView);
        void Unregister(EntityId entityId);
    }
    public class EntityReferenceService : IEntityReferenceService
    {
        private readonly DataResolver<EntityId, EntityView> _idToViewResolver;

        public EntityReferenceService()
        {
            _idToViewResolver = new DataResolver<EntityId, EntityView>();
        }
        public EntityView GetEntityViewBy(EntityId entityId)
        {
            return _idToViewResolver.Get(entityId);
        }
        public void Register(EntityId entityId, EntityView entityView)
        {
            if (_idToViewResolver.Contains(entityId))
            {
                throw new Exception("And entity with id {entityId} is already registered.");
            }
            _idToViewResolver.Set(entityId, entityView);
        }
        public void Unregister(EntityId entityId)
        {
            _idToViewResolver.Remove(entityId);
        }
    }
}
