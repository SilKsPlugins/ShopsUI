using Cysharp.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using ShopsUI.Commands.Items;
using ShopsUI.Database;
using System;

namespace ShopsUI.Commands
{
    [Command("reload", Priority = Priority.High)]
    [CommandDescription("Reloads the shops from the database.")]
    [CommandParent(typeof(CShop))]
    public class CShopReload : UnturnedCommand
    {
        private readonly ShopsDbContext _dbContext;
        private readonly IStringLocalizer _stringLocalizer;

        public CShopReload(ShopsDbContext dbContext,
            IStringLocalizer stringLocalizer,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = dbContext;
            _stringLocalizer = stringLocalizer;
        }

        protected override async UniTask OnExecuteAsync()
        {
            await _dbContext.ItemShops.LoadAsync();
            await _dbContext.VehicleShops.LoadAsync();

            await PrintAsync(_stringLocalizer["commands:success:shop_reloaded"]);
        }
    }
}
