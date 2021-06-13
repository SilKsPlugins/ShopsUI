using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class ShopManager : IShopManager
    {
        private readonly ShopsDbContext _dbContext;
        private readonly IOpenModComponent _openModComponent;

        public ShopManager(ShopsDbContext dbContext,
            IOpenModComponent openModComponent)
        {
            _dbContext = dbContext;
            _openModComponent = openModComponent;
        }

        public async Task<ICollection<IItemShopData>> GetItemShopDatasAsync(
            Func<IQueryable<IItemShopData>, IQueryable<IItemShopData>> query)
        {
            var queryable = _dbContext.ItemShops
                .OrderByDescending(x => x.Order)
                .ThenBy(x => x.ItemId);

            return await query(queryable).ToListAsync();
        }

        public async Task<ICollection<IVehicleShopData>> GetVehicleShopDatasAsync(
            Func<IQueryable<IVehicleShopData>, IQueryable<IVehicleShopData>> query)
        {
            var queryable = _dbContext.VehicleShops
                .OrderByDescending(x => x.Order)
                .ThenBy(x => x.VehicleId);

            return await query(queryable).ToListAsync();
        }

        public async Task<ItemShopModel?> GetItemShopData(ushort id)
        {
            return await _dbContext.ItemShops.FindAsync((int) id);
        }

        async Task<IItemShopData?> IShopManager.GetItemShopData(ushort id) =>
            await GetItemShopData(id);

        public async Task<VehicleShopModel?> GetVehicleShopData(ushort id)
        {
            return await _dbContext.VehicleShops.FindAsync((int) id);
        }

        async Task<IVehicleShopData?> IShopManager.GetVehicleShopData(ushort id) =>
            await GetVehicleShopData(id);

        public async Task<IItemShop?> GetItemShop(ushort id)
        {
            var data = await GetItemShopData(id);

            return data == null
                ? null
                : ActivatorUtilitiesEx.CreateInstance<ItemShop>(_openModComponent.LifetimeScope, data);
        }

        public async Task<IVehicleShop?> GetVehicleShop(ushort id)
        {
            var data = await GetVehicleShopData(id);

            return data == null
                ? null
                : ActivatorUtilitiesEx.CreateInstance<VehicleShop>(_openModComponent.LifetimeScope, data);
        }
        
        public async Task<IItemShop> AddItemShopBuyable(ushort id, decimal price)
        {
            var data = await _dbContext.ItemShops.FindAsync(id);

            if (data == null)
            {
                data = new ItemShopModel
                {
                    ItemId = id,
                    BuyPrice = price
                };

                await _dbContext.ItemShops.AddAsync(data);
            }
            else
            {
                data.BuyPrice = price;

                _dbContext.ItemShops.Update(data);
            }

            await _dbContext.SaveChangesAsync();

            return ActivatorUtilitiesEx.CreateInstance<ItemShop>(_openModComponent.LifetimeScope, data);
        }

        public async Task<IItemShop> AddItemShopSellable(ushort id, decimal price)
        {
            var data = await _dbContext.ItemShops.FindAsync(id);

            if (data == null)
            {
                data = new ItemShopModel
                {
                    ItemId = id,
                    SellPrice = price
                };

                await _dbContext.ItemShops.AddAsync(data);
            }
            else
            {
                data.SellPrice = price;

                _dbContext.ItemShops.Update(data);
            }

            await _dbContext.SaveChangesAsync();

            return ActivatorUtilitiesEx.CreateInstance<ItemShop>(_openModComponent.LifetimeScope, data);
        }

        public async Task<IVehicleShop> AddVehicleShopBuyable(ushort id, decimal price)
        {
            var data = await _dbContext.VehicleShops.FindAsync(id);

            if (data == null)
            {
                data = new VehicleShopModel
                {
                    VehicleId = id,
                    BuyPrice = price
                };

                await _dbContext.VehicleShops.AddAsync(data);
            }
            else
            {
                data.BuyPrice = price;

                _dbContext.VehicleShops.Update(data);
            }

            await _dbContext.SaveChangesAsync();

            return ActivatorUtilitiesEx.CreateInstance<VehicleShop>(_openModComponent.LifetimeScope, data);
        }

        public async Task<bool> RemoveItemShopBuyable(ushort id)
        {
            var data = await _dbContext.ItemShops.FindAsync((int)id);

            if (data?.BuyPrice == null) return false;

            data.BuyPrice = null;

            if (data.SellPrice == null)
            {
                _dbContext.ItemShops.Remove(data);
            }
            else
            {
                _dbContext.ItemShops.Update(data);
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveItemShopSellable(ushort id)
        {
            var data = await _dbContext.ItemShops.FindAsync((int)id);

            if (data?.SellPrice == null) return false;

            data.SellPrice = null;

            if (data.BuyPrice == null)
            {
                _dbContext.ItemShops.Remove(data);
            }
            else
            {
                _dbContext.ItemShops.Update(data);
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveVehicleShopBuyable(ushort id)
        {
            var data = await _dbContext.VehicleShops.FindAsync((int)id);

            if (data == null) return false;

            _dbContext.VehicleShops.Remove(data);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
