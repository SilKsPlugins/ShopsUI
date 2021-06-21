using Cysharp.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using ShopsUI.Database;
using System;

[assembly: PluginMetadata("ShopsUI", DisplayName = "ShopsUI")]
namespace ShopsUI
{
    public class ShopsUIPlugin : OpenModUnturnedPlugin
    {
        private readonly ShopsDbContext _dbContext;

        public ShopsUIPlugin(
            ShopsDbContext dbContext,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = dbContext;
        }

        protected override async UniTask OnLoadAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}
