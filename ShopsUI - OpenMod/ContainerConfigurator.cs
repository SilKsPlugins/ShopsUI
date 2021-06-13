using Autofac;
using OpenMod.API.Ioc;
using SilK.OpenMod.EntityFrameworkCore;

namespace ShopsUI
{
    public class ContainerConfigurator : IContainerConfigurator
    {
        public void ConfigureContainer(IOpenModServiceConfigurationContext openModStartupContext,
            ContainerBuilder containerBuilder)
        {
            containerBuilder.AddPomeloMySqlConnectorResolver();
        }
    }
}
