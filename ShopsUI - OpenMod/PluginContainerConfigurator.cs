using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using ShopsUI.Database;

namespace ShopsUI
{
    public class PluginContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddEntityFrameworkCoreMySql();
            context.ContainerBuilder.AddDbContext<ShopsDbContext>(ServiceLifetime.Transient);
        }
    }
}
