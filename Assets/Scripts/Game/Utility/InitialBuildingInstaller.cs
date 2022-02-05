using System;
using Game.Entity;
using Simulation.Construction;
using UnityEngine;
using Zenject;

namespace Game.Utility
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public class InitialBuildingInstaller : MonoBehaviour
	{
		[Inject] private IConstructionService _constructionService;

		private void Awake()
		{
			var initialBuildings = FindObjectsOfType<EntityView>();
			foreach (var initialBuilding in initialBuildings)
			{
				if (initialBuilding is BuildingView buildingView)
				{
					
				}

			}
		}
	}
}
