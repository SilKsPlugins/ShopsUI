using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using ShopsUI.API.Shops.Items.Whitelist;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Items.Whitelist
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class ItemShopWhitelistEditor : IItemShopWhitelistEditor
    {
        private readonly ShopsDbContext _dbContext;
        private readonly IItemShopDirectory _shopDirectory;

        public ItemShopWhitelistEditor(ShopsDbContext dbContext, IItemShopDirectory shopDirectory)
        {
            _dbContext = dbContext;
            _shopDirectory = shopDirectory;
        }

        public async Task<bool> AddWhitelist(ushort id, string permission)
        {
            var data = await _dbContext.ItemShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            if (data.AuthGroups!.Any(x => x.IsWhitelist && x.Permission == permission))
            {
                return false;
            }

            await _dbContext.ItemGroups.AddAsync(new ItemGroupModel
            {
                Permission = permission,
                IsWhitelist = true,
                ItemShop = data
            });

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }

        public async Task<bool> RemoveWhitelist(ushort id, string permission)
        {
            var group = await _dbContext.ItemGroups.FirstOrDefaultAsync(x =>
                x.ItemShopItemId == id && x.Permission == permission && x.IsWhitelist);

            if (group == null)
            {
                return false;
            }

            _dbContext.ItemGroups.Remove(group);

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }


        public async Task<bool> AddBlacklist(ushort id, string permission)
        {
            var data = await _dbContext.ItemShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            if (data.AuthGroups!.Any(x => !x.IsWhitelist && x.Permission == permission))
            {
                return false;
            }

            await _dbContext.ItemGroups.AddAsync(new ItemGroupModel
            {
                Permission = permission,
                IsWhitelist = false,
                ItemShop = data
            });

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }

        public async Task<bool> RemoveBlacklist(ushort id, string permission)
        {
            var group = await _dbContext.ItemGroups.FirstOrDefaultAsync(x =>
                x.ItemShopItemId == id && x.Permission == permission && !x.IsWhitelist);

            if (group == null)
            {
                return false;
            }

            _dbContext.ItemGroups.Remove(group);

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }
    }
}
