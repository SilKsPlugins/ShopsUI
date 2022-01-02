using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Items
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class ItemShopEditor : IItemShopEditor
    {
        private readonly ShopsDbContext _dbContext;
        private readonly IItemShopDirectory _shopDirectory;

        public ItemShopEditor(ShopsDbContext dbContext, IItemShopDirectory shopDirectory)
        {
            _dbContext = dbContext;
            _shopDirectory = shopDirectory;
        }

        public async Task<IItemShopData> AddItemShopBuyable(ushort id, decimal price)
        {
            var data = await _dbContext.ItemShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                data = new ItemShopModel
                {
                    Id = id,
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

            _shopDirectory.InvalidateFireAndForget();

            return data;
        }

        public async Task<IItemShopData> AddItemShopSellable(ushort id, decimal price)
        {
            var data = await _dbContext.ItemShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                data = new ItemShopModel
                {
                    Id = id,
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

            _shopDirectory.InvalidateFireAndForget();

            return data;
        }

        public async Task<bool> RemoveItemShopBuyable(ushort id)
        {
            var data = await _dbContext.ItemShops.FindAsync(id);

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

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }

        public async Task<bool> RemoveItemShopSellable(ushort id)
        {
            var data = await _dbContext.ItemShops.FindAsync(id);

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

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }
    }
}
