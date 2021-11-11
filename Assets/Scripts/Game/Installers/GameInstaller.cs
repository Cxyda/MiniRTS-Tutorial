using Game.InputHandling;
using Game.Selection;
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
			Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle();
			Container.BindInterfacesAndSelfTo<SelectionService>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CameraRaycastHandler>().AsSingle().NonLazy();
		}
	}
}