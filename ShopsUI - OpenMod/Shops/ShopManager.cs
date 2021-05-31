using Autofac;
using Microsoft.EntityFrameworkCore;
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
using SilK.Unturned.Extras.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ShopManager : IShopManager
    {
        private readonly IPluginAccessor<ShopsUIPlugin> _pluginAccessor;
        private readonly IActionDispatcher _actionDispatcher;

        public ShopManager(
            IPluginAccessor<ShopsUIPlugin> pluginAccessor,
            IActionDispatcher actionDispatcher)
        {
            _pluginAccessor = pluginAccessor;
            _actionDispatcher = actionDispatcher;
        }

        private ILifetimeScope GetPluginScope() => _pluginAccessor.Instance?.LifetimeScope ??
                                                   throw new Exception("ShopsUI plugin is not loaded");

        private ShopsDbContext? _cachedDbContext;

        private ShopsDbContext GetDbContext() => _cachedDbContext ??= GetPluginScope().Resolve<ShopsDbContext>();

        public async Task<ICollection<IItemShopData>> GetItemShopDatasAsync(
            Func<IQueryable<IItemShopData>, IQueryable<IItemShopData>> query)
        {
            return await _actionDispatcher.Enqueue(async () =>
            {
                var queryable = GetDbContext().ItemShops
                    .OrderByDescending(x => x.Order)
                    .ThenBy(x => x.ItemId);

                return await query(queryable).ToListAsync();
            });
        }

        public async Task<ICollection<IVehicleShopData>> GetVehicleShopDatasAsync(
            Func<IQueryable<IVehicleShopData>, IQueryable<IVehicleShopData>> query)
        {
            return await _actionDispatcher.Enqueue(async () =>
            {
                var queryable = GetDbContext().VehicleShops
                    .OrderByDescending(x => x.Order)
                    .ThenBy(x => x.VehicleId);

                return await query(queryable).ToListAsync();
            });
        }

        public Task<ItemShopModel?> GetItemShopData(ushort id)
        {
            return _actionDispatcher.Enqueue(async () =>
                (ItemShopModel?) await GetDbContext().ItemShops.FindAsync((int)id));
        }

        async Task<IItemShopData?> IShopManager.GetItemShopData(ushort id) =>
            await GetItemShopData(id);

        public Task<VehicleShopModel?> GetVehicleShopData(ushort id)
        {
            return _actionDispatcher.Enqueue(async () =>
                (VehicleShopModel?) await GetDbContext().VehicleShops.FindAsync((int)id));
        }

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
            return await _actionDispatcher.Enqueue(async () =>
            {
                var dbContext = GetDbContext();

                var data = await dbContext.ItemShops.FindAsync(id);

                if (data == null)
                {
                    data = new ItemShopModel
                    {
                        ItemId = id,
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
            });
        }

        public async Task<IItemShop> AddItemShopSellable(ushort id, decimal price)
        {
            return await _actionDispatcher.Enqueue(async () =>
            {
                var dbContext = GetDbContext();

                var data = await dbContext.ItemShops.FindAsync(id);

                if (data == null)
                {
                    data = new ItemShopModel
                    {
                        ItemId = id,
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
            });
        }

        public async Task<IVehicleShop> AddVehicleShopBuyable(ushort id, decimal price)
        {
            return await _actionDispatcher.Enqueue(async () =>
            {
                var dbContext = GetDbContext();

                var data = await dbContext.VehicleShops.FindAsync(id);

                if (data == null)
                {
                    data = new VehicleShopModel
                    {
                        VehicleId = id,
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
            });
        }

        public Task<bool> RemoveItemShopBuyable(ushort id)
        {
            return _actionDispatcher.Enqueue(async () =>
            {
                var dbContext = GetDbContext();

                var data = await dbContext.ItemShops.FindAsync((int)id);
                
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
            });
        }

        public Task<bool> RemoveItemShopSellable(ushort id)
        {
            return _actionDispatcher.Enqueue(async () =>
            {
                var dbContext = GetDbContext();

                var data = await dbContext.ItemShops.FindAsync((int)id);

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
            });
        }

        public Task<bool> RemoveVehicleShopBuyable(ushort id)
        {
            return _actionDispatcher.Enqueue(async () =>
            {
                var dbContext = GetDbContext();

                var data = await dbContext.VehicleShops.FindAsync((int)id);

                if (data == null) return false;

                dbContext.VehicleShops.Remove(data);

                await dbContext.SaveChangesAsync();

                return true;
            });
        }
    }
}
