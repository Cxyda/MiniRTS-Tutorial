using Simulation.Data;

namespace Simulation.Construction
{
	/// <summary>
	/// Model classes implement one or more data resolver which is able to resolve data to a unique key.
	/// A data resolver stores key-value data. There may be more than one resolver e.g. to speed up often needed lookups 
	/// </summary>
	internal class ConstructionModel
	{
		internal readonly IDataResolver<EntityId, ConstructionVO> Constructions;
		internal readonly IDataResolver<EntityId, FactoryVO> Factories;

		internal ConstructionModel()
		{
			Constructions = new DataResolver<EntityId, ConstructionVO>();
			Factories = new DataResolver<EntityId, FactoryVO>();
		}
	}
}