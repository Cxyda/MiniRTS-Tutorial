using Simulation.Data;

namespace Simulation.Construction
{
	/// <summary>
	/// This struct is responsible for storing construction data of an entity.
	/// The ConstructionPointsInvested is stored as a ushort instead of a float since floating point calculation may differ
	/// on different machines.
	/// </summary>
	internal struct ConstructionVO
	{
		internal const byte ConstructionPointsScaleFactor = 10;

		internal EntityId EntityId;

		/// <summary>
		/// The amount of construction points (work) that has been invested already.
		/// The value is scaled by a factor of 10 to be able to reflect construction speed boosts.
		/// E.g. a construction speed of 1 would add then 10 construction points. With a 10% speed boost it would be 11.
		/// This means the smallest speed boost can be 10%
		/// If this value / 10 reaches the defined value in the GD config, the construction is finished.
		/// </summary>
		internal ushort ConstructionPointsInvested;
		
		internal ConstructionVO(EntityId entityEntityId, ushort constructionPointsInvested = 0)
		{
			EntityId = entityEntityId;
			ConstructionPointsInvested = constructionPointsInvested;
		}
	}
}