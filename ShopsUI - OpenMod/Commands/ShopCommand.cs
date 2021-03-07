﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.Core.Ioc;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Users;
using ShopsUI.API;
using System;
using System.Threading.Tasks;

namespace ShopsUI.Commands
{
    [DontAutoRegister]
    public abstract class ShopCommand : UnturnedCommand
    {
        protected readonly IStringLocalizer StringLocalizer;
        protected readonly IItemDirectory ItemDirectory;
        protected readonly IVehicleDirectory VehicleDirectory;
        protected readonly IShopManager ShopManager;
        protected readonly IEconomyProvider EconomyProvider;

        protected ShopCommand(
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            StringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer>();
            ItemDirectory = serviceProvider.GetRequiredService<IItemDirectory>();
            VehicleDirectory = serviceProvider.GetRequiredService<IVehicleDirectory>();
            ShopManager = serviceProvider.GetRequiredService<IShopManager>();
            EconomyProvider = serviceProvider.GetRequiredService<IEconomyProvider>();
        }

        protected void ValidatePrice(decimal price)
        {
            if (price < 0)
                throw new UserFriendlyException(StringLocalizer["commands:errors:invalid_price", new {Price = price}]);
        }

        protected void ValidateAmount(int amount)
        {
            if (amount < 1)
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:invalid_amount", new { Amount = amount }]);
        }

        protected async Task<IItemAsset> GetItemAsset(int index)
        {
            var idOrName = await Context.Parameters.GetAsync<string>(index);

            if (string.IsNullOrWhiteSpace(idOrName))
                throw new UserFriendlyException(StringLocalizer["commands:errors:invalid_item_id", new { IdOrName = idOrName }]);
            
            IItemAsset? asset;

            if (ushort.TryParse(idOrName, out _))
            {
                asset = await ItemDirectory.FindByIdAsync(idOrName);

                if (asset != null) return asset;
            }

            asset = await ItemDirectory.FindByNameAsync(idOrName, false);

            if (asset == null || asset is UnturnedItemAsset unturnedItemAsset && unturnedItemAsset.ItemAsset.isPro)
                throw new UserFriendlyException(StringLocalizer["commands:errors:invalid_item_id",
                    new {IdOrName = idOrName}]);

            return asset;
        }

        protected async Task<IVehicleAsset> GetVehicleAsset(int index)
        {
            var idOrName = await Context.Parameters.GetAsync<string>(index);

            if (string.IsNullOrWhiteSpace(idOrName))
                throw new UserFriendlyException(StringLocalizer["commands:errors:invalid_vehicle_id", new { IdOrName = idOrName }]);

            return await VehicleDirectory.FindByIdAsync(idOrName)
                   ?? await VehicleDirectory.FindByNameAsync(idOrName, false)
                   ?? throw new UserFriendlyException(
                       StringLocalizer["commands:errors:invalid_vehicle_id", new {IdOrName = idOrName}]);
        }

        protected async Task<decimal> GetPrice(int index)
        {
            var price = await Context.Parameters.GetAsync<decimal>(index);
            ValidatePrice(price);
            return price;
        }

        protected async Task<int> GetAmount(int index)
        {
            var amount = await Context.Parameters.GetAsync<int>(index, 1);
            ValidateAmount(amount);
            return amount;
        }

        protected UnturnedUser GetUnturnedUser() => (UnturnedUser) Context.Actor;
    }
}
