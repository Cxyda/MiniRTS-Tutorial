using Zenject;

namespace Simulation.Simulation
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public class SimulationInstaller : Installer<SimulationInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<EntityIdFactory>().AsSingle();

			Container.BindInterfacesAndSelfTo<SimulationQueueService>().AsSingle();
			Container.Bind<SimulationLocator>().AsSingle();
			Container.BindInterfacesTo<SimulationService>().AsSingle().NonLazy();
		}
	}
}