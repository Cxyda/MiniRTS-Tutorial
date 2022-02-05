using Game.AssetLoading;
using Game.BuildMode;
using Game.EntityReference;
using Game.Factory;
using Game.InputHandling;
using Game.Selection;
using Game.Utility;
using Simulation.Construction;
using Simulation.EntityTypeMapping;
using Simulation.GameTime;
using Simulation.Location;
using Simulation.Simulation;
using Zenject;

namespace Game.Installers
{
	/// <summary>
	/// This class installs the core game systems
	/// </summary>
	public class GameInstaller : MonoInstaller<GameInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PrefabFactory>().AsSingle();
			Container.BindInterfacesAndSelfTo<AssetLoadService>().AsSingle();

			Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle();
			Container.BindInterfacesAndSelfTo<SelectionService>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CameraRaycastHandler>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<BuildModeService>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<FactoryService>().AsSingle().NonLazy();

			Container.BindInterfacesAndSelfTo<EntityReferenceService>().AsSingle().NonLazy();
			
			SimulationInstaller.Install(Container);
			TimeServiceInstaller.Install(Container);
			ConstructionInstaller.Install(Container);
			LocationInstaller.Install(Container);
			EntityTypeInstaller.Install(Container);
		}
	}
}