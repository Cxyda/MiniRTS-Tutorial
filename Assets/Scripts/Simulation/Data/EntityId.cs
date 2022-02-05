using System;

namespace Simulation.Data
{
	/// <summary>
	/// TODO: 
	/// </summary>
	[Serializable]
	public struct EntityId
	{
		public static EntityId Invalid = default;

		public readonly int ID;

		public EntityId(int id)
		{
			ID = id;
		}
		
		public bool Equals(EntityId other)
		{
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			return obj is EntityId other && other.ID == ID;
		}

		public override int GetHashCode()
		{
			return ID;
		}

		public static bool operator ==(EntityId a, EntityId b)
		{
			return a.ID == b.ID;
		}
		public static bool operator !=(EntityId a, EntityId b)
		{
			return !(a == b);
		}
	}
}