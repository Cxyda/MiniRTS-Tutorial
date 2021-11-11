using Game.AssetLoading;
using Game.BuildMode;
using Game.InputHandling;
using Game.Selection;
using Game.Utility;
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
		}
	}
}