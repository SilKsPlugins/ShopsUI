using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Ioc;
using ShopsUI.API;
using ShopsUI.API.Items;
using ShopsUI.API.Vehicles;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using ShopsUI.Shops.Items;
using ShopsUI.Shops.Vehicles;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ShopManager : IShopManager
    {
        private readonly IPluginAccessor<ShopsUIPlugin> _pluginAccessor;

        public ShopManager(
            IPluginAccessor<ShopsUIPlugin> pluginAccessor)
        {
            _pluginAccessor = pluginAccessor;
        }

        private ILifetimeScope GetPluginScope() => _pluginAccessor.Instance?.LifetimeScope ??
                                                   throw new Exception("ShopsUI plugin is not loaded");

        private ShopsDbContext? _cachedDbContext;

        private ShopsDbContext GetDbContext() => _cachedDbContext ??= GetPluginScope().Resolve<ShopsDbContext>();

        public IQueryable<IItemShopData> GetItemShopDatas() =>
            GetDbContext().ItemShops.AsQueryable()
                .OrderByDescending(x => x.Order)
                .ThenBy(x => x.ItemShopId);

        public IQueryable<IVehicleShopData> GetVehicleShopDatas() =>
            GetDbContext().VehicleShops.AsQueryable()
                .OrderByDescending(x => x.Order)
                .ThenBy(x => x.VehicleShopId);

        public async Task<ItemShopModel?> GetItemShopData(ushort id) =>
            await GetDbContext().ItemShops.FindAsync((int) id);

        async Task<IItemShopData?> IShopManager.GetItemShopData(ushort id) =>
            await GetItemShopData(id);

        public async Task<VehicleShopModel?> GetVehicleShopData(ushort id) =>
            await GetDbContext().VehicleShops.FindAsync((int) id);

        async Task<IVehicleShopData?> IShopManager.GetVehicleShopData(ushort id) =>
            await GetVehicleShopData(id);

        public async Task<IItemShop?> GetItemShop(ushort id)
        {
            var data = await GetItemShopData(id);

            return data == null
                ? null
                : ActivatorUtilitiesEx.CreateInstance<ItemShop>(GetPluginScope(), data);
        }

        public async Task<IVehicleShop?> GetVehicleShop(ushort id)
        {
            var data = await GetVehicleShopData(id);

            return data == null
                ? null
                : ActivatorUtilitiesEx.CreateInstance<VehicleShop>(GetPluginScope(), data);
        }
        
        public async Task<IItemShop> AddItemShopBuyable(ushort id, decimal price)
        {
            var data = await GetItemShopData(id);

            var dbContext = GetDbContext();

            if (data == null)
            {
                data = new ItemShopModel
                {
                    ItemShopId = id,
                    BuyPrice = price
                };

                await dbContext.ItemShops.AddAsync(data);
            }
            else
            {
                data.BuyPrice = price;

                dbContext.ItemShops.Update(data);
            }

            await dbContext.SaveChangesAsync();

            return ActivatorUtilitiesEx.CreateInstance<ItemShop>(GetPluginScope(), data);
        }

        public async Task<IItemShop> AddItemShopSellable(ushort id, decimal price)
        {
            var data = await GetItemShopData(id);

            var dbContext = GetDbContext();

            if (data == null)
            {
                data = new ItemShopModel
                {
                    ItemShopId = id,
                    SellPrice = price
                };

                await dbContext.ItemShops.AddAsync(data);
            }
            else
            {
                data.SellPrice = price;

                dbContext.ItemShops.Update(data);
            }

            await dbContext.SaveChangesAsync();

            return ActivatorUtilitiesEx.CreateInstance<ItemShop>(GetPluginScope(), data);
        }

        public async Task<IVehicleShop> AddVehicleShopBuyable(ushort id, decimal price)
        {
            var data = await GetVehicleShopData(id);

            var dbContext = GetDbContext();

            if (data == null)
            {
                data = new VehicleShopModel
                {
                    VehicleShopId = id,
                    BuyPrice = price
                };

                await dbContext.VehicleShops.AddAsync(data);
            }
            else
            {
                data.BuyPrice = price;

                dbContext.VehicleShops.Update(data);
            }

            await dbContext.SaveChangesAsync();

            return ActivatorUtilitiesEx.CreateInstance<VehicleShop>(GetPluginScope(), data);
        }

        public async Task<bool> RemoveItemShopBuyable(ushort id)
        {
            var data = await GetItemShopData(id);

            var dbContext = GetDbContext();

            if (data?.BuyPrice == null) return false;

            data.BuyPrice = null;

            if (data.SellPrice == null)
            {
                dbContext.ItemShops.Remove(data);
            }
            else
            {
                dbContext.ItemShops.Update(data);
            }

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveItemShopSellable(ushort id)
        {
            var data = await GetItemShopData(id);

            var dbContext = GetDbContext();

            if (data?.SellPrice == null) return false;

            data.SellPrice = null;

            if (data.BuyPrice == null)
            {
                dbContext.ItemShops.Remove(data);
            }
            else
            {
                dbContext.ItemShops.Update(data);
            }

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveVehicleShopBuyable(ushort id)
        {
            var data = await GetVehicleShopData(id);

            var dbContext = GetDbContext();

            if (data == null) return false;

            dbContext.VehicleShops.Remove(data);

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
