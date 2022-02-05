using System;
using Game.AssetLoading;
using Game.Entity;
using Game.Utility;
using Simulation.Construction;
using Simulation.Data;
using Simulation.EntityTypeMapping;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.Factory
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public class FactoryView : MonoBehaviour
	{
		[Inject] private IConstructionService _constructionService;
		[Inject] private IEntityTypeDataProvider _entityTypeDataProvider;
		[Inject] private IPrefabFactory _prefabFactory;

		[SerializeField] protected BuildingView BuildingView;
		[SerializeField] protected Vector3[] SpawnPoints;

		private void Awake()
		{
			if(BuildingView == null) return;
			_constructionService.ProductionFinishedEvent += OnProductionFinished;
			_constructionService.ConstructionFinishedEvent += OnConstructionFinished;
		}

		private void OnDestroy()
		{
			_constructionService.ProductionFinishedEvent -= OnProductionFinished;
			_constructionService.ConstructionFinishedEvent -= OnConstructionFinished;
		}

		private void OnProductionFinished(EntityId entityId)
		{
			if(_constructionService.IsUnderConstruction(BuildingView.EntityId)) return;

			// TODO: Handle spawn unit
			var entityTYpe = _entityTypeDataProvider.Get(entityId);

			_prefabFactory.CreateGameObject(AssetType.GameObject, entityTYpe, OnCreated, GetEmptySpawnPoint());
			void OnCreated(GameObject gameObject)
			{
				// TODO:
			}
		}
		private void OnConstructionFinished(EntityId entityId)
		{
			if(BuildingView.EntityId != entityId) return;
			
			_constructionService.ConstructionFinishedEvent -= OnConstructionFinished;
			_constructionService.CreateFactory(BuildingView.EntityId);

		}
		private void OnDrawGizmosSelected()
		{
			foreach (var spawnPoint in SpawnPoints)
			{
				Handles.DrawWireDisc(transform.localPosition + spawnPoint, Vector3.up, 0.25f);
			}
		}

		Vector3 GetEmptySpawnPoint()
		{
			return SpawnPoints[0];
		}
	}
}
