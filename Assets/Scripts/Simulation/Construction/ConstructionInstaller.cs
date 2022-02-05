using Zenject;

namespace Simulation.Construction
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public class ConstructionInstaller : Installer<ConstructionInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<ConstructionModel>().AsSingle();
			Container.BindInterfacesTo<ConstructionService>().AsSingle();
		}
	}
}