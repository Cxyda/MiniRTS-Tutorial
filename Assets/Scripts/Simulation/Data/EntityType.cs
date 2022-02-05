namespace Simulation.Data
{
	/// <summary>
	/// This enum contains all entity types in the game. Everything in the game that needs to be identified
	/// in the game can have it's own type.
	/// It does not matter if that is a GameObject, an Animation, an Icon or a Level etc.
	/// </summary>
	public enum EntityType : short
	{
		Invalid = -1,
		None = 0,
		BuildingA = 1,
		BuildingB = 2,
		BuildingC = 3,
		DepositA = 500,
		DepositB = 501,
		DepositC = 502,
		UnitA = 1001,
		UnitB = 1002,
		UnitC = 1003,
	}
}