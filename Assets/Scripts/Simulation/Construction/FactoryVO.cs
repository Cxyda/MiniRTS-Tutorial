using System.Collections;
using System.Collections.Generic;
using Simulation.Data;

namespace Simulation.Construction
{
	/// <summary>
	/// TODO: 
	/// </summary>
	internal struct FactoryVO
	{
		// EntityId of the factory
		internal EntityId EntityId;
		// EntityId of the producing "unit"
		internal EntityId ProductionEntityId;

		internal Queue<EntityType> ProductionQueue;

		public ushort ProductionPointsInvested
		{
			get;
			set;
		}
	}
}
