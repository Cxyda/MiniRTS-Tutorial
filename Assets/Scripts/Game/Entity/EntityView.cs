using Simulation.Data;
using UnityEngine;

namespace Game.Entity
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public abstract class EntityView : MonoBehaviour
	{
		public EntityId EntityId;

		[SerializeField] protected EntityType entityType;
	}
}