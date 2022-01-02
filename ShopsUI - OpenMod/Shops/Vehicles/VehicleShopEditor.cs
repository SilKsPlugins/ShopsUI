using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Vehicles
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class VehicleShopEditor : IVehicleShopEditor
    {
        private readonly ShopsDbContext _dbContext;
        private readonly IVehicleShopDirectory _shopDirectory;

        public VehicleShopEditor(ShopsDbContext dbContext, IVehicleShopDirectory shopDirectory)
        {
            _dbContext = dbContext;
            _shopDirectory = shopDirectory;
        }

        public async Task<IVehicleShopData> AddVehicleShopBuyable(ushort id, decimal price)
        {
            var data = await _dbContext.VehicleShops.Include(x => x.AuthGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                data = new VehicleShopModel
                {
                    Id = id,
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

            _shopDirectory.InvalidateFireAndForget();

            return data;
        }

        public async Task<bool> RemoveVehicleShopBuyable(ushort id)
        {
            var data = await _dbContext.VehicleShops.FindAsync(id);

            if (data == null)
            {
                return false;
            }

            _dbContext.VehicleShops.Remove(data);

            await _dbContext.SaveChangesAsync();

            _shopDirectory.InvalidateFireAndForget();

            return true;
        }
    }
}
