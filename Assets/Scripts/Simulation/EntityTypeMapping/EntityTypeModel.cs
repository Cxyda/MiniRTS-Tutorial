using Simulation.Data;

namespace Simulation.EntityTypeMapping
{
    /// <summary>
    /// Model classes implement one or more data resolver which is able to resolve data to a unique key.
    /// A data resolver stores key-value data. There may be more than one resolver e.g. to speed up often needed lookups 
    /// </summary>
    internal class EntityTypeModel
    {
        internal readonly IDataResolver<EntityId, EntityTypeVO> DataResolver;

        internal EntityTypeModel()
        {
            DataResolver = new DataResolver<EntityId, EntityTypeVO>();
        }
    }
}
