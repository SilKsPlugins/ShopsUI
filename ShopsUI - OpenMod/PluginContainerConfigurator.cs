using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.MySql.Extensions;
using ShopsUI.Database;

namespace ShopsUI
{
    public class PluginContainerConfigurator : IPluginContainerConfigurator
{
    public void ConfigureContainer(IPluginServiceConfigurationContext context)
    {
        context.ContainerBuilder.AddMySqlDbContext<ShopsDbContext>();
    }
}
}
