using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.API.Shops.Vehicles.Whitelist;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Vehicles.Whitelist
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class VehicleShopWhitelistEditor : IVehicleShopWhitelistEditor
    {
        private readonly ShopsDbContext _dbContext;
        private readonly IVehicleShopDirectory _shopDirectory;

        public VehicleShopWhitelistEditor(ShopsDbContext dbContext, IVehicleShopDirectory shopDirectory)
        {
            _dbContext = dbContext;
            _shopDirectory = shopDirectory;
        }

        public async Task<bool> AddWhitelist(ushort id, string permission)
        {
            var data = await _dbContext.VehicleShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            if (data.AuthGroups!.Any(x => x.IsWhitelist && x.Permission == permission))
            {
                return false;
            }

            await _dbContext.VehicleGroups.AddAsync(new VehicleGroupModel
            {
                Permission = permission,
                IsWhitelist = true,
                VehicleShop = data
            });

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }

        public async Task<bool> RemoveWhitelist(ushort id, string permission)
        {
            var group = await _dbContext.VehicleGroups.FirstOrDefaultAsync(x =>
                x.VehicleShopVehicleId == id && x.Permission == permission && x.IsWhitelist);

            if (group == null)
            {
                return false;
            }

            _dbContext.VehicleGroups.Remove(group);

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }


        public async Task<bool> AddBlacklist(ushort id, string permission)
        {
            var data = await _dbContext.VehicleShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            if (data.AuthGroups!.Any(x => !x.IsWhitelist && x.Permission == permission))
            {
                return false;
            }

            await _dbContext.VehicleGroups.AddAsync(new VehicleGroupModel
            {
                Permission = permission,
                IsWhitelist = false,
                VehicleShop = data
            });

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }

        public async Task<bool> RemoveBlacklist(ushort id, string permission)
        {
            var group = await _dbContext.VehicleGroups.FirstOrDefaultAsync(x =>
                x.VehicleShopVehicleId == id && x.Permission == permission && !x.IsWhitelist);

            if (group == null)
            {
                return false;
            }

            _dbContext.VehicleGroups.Remove(group);

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }
    }
}
