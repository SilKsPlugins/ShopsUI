extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.MySql.Extensions;
using ShopsUI.Database;

namespace ShopsUI
{
    [UsedImplicitly]
    public class PluginContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddMySqlDbContext<ShopsDbContext>();
        }
    }
}
