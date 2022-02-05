using Simulation.Construction;
using Zenject;

namespace Simulation.EntityTypeMapping
{
    /// <summary>
    /// TODO: 
    /// </summary>
    public class EntityTypeInstaller : Installer<EntityTypeInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<EntityTypeModel>().AsSingle();
            Container.BindInterfacesTo<EntityTypeService>().AsSingle();
        }
    }
}
