using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Vehicles;
using SDG.Unturned;
using ShopsUI.API.Vehicles;
using ShopsUI.Database.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Vehicles
{
    public class VehicleShop : IVehicleShop
    {
        private readonly IEconomyProvider _economyProvider;
        private readonly IVehicleDirectory _vehicleDirectory;
        private readonly IVehicleSpawner _vehicleSpawner;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IConfiguration _configuration;
        private readonly IOpenModComponent _openModComponent;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPermissionRegistry _permissionRegistry;
        private readonly ILogger<VehicleShop> _logger;

        public VehicleShopModel ShopData { get; }

        IVehicleShopData IVehicleShop.ShopData => ShopData;

        public const string PermissionFormat = "groups.{0}";

        public VehicleShop(IEconomyProvider economyProvider,
            IVehicleDirectory vehicleDirectory,
            IVehicleSpawner vehicleSpawner,
            IStringLocalizer stringLocalizer,
            IConfiguration configuration,
            IOpenModComponent openModComponent,
            IPermissionChecker permissionChecker,
            IPermissionRegistry permissionRegistry,
            ILogger<VehicleShop> logger,
            VehicleShopModel vehicleShopModel)
        {
            _economyProvider = economyProvider;
            _vehicleDirectory = vehicleDirectory;
            _vehicleSpawner = vehicleSpawner;
            _stringLocalizer = stringLocalizer;
            _configuration = configuration;
            _openModComponent = openModComponent;
            _permissionChecker = permissionChecker;
            _permissionRegistry = permissionRegistry;
            _logger = logger;

            ShopData = vehicleShopModel;
        }

        private async Task<UnturnedVehicleAsset> GetVehicleAsset()
        {
            var id = ShopData.VehicleId.ToString();

            var vehicleAsset =
                (await _vehicleDirectory.GetVehicleAssetsAsync()).FirstOrDefault(x => x.VehicleAssetId.Equals(id))
                as UnturnedVehicleAsset;

            if (vehicleAsset == null)
                throw new Exception($"Vehicle id '{id}' has no asset");

            return vehicleAsset;
        }

        public async Task<decimal> Buy(UnturnedUser user)
        {
            var vehicleAsset = await GetVehicleAsset();

            var balance = await _economyProvider.UpdateBalanceAsync(user.Id, user.Type, -ShopData.BuyPrice,
                _stringLocalizer["transactions:vehicles:bought",
                    new
                    {
                        VehicleAsset = vehicleAsset,
                        Price = ShopData.BuyPrice,
                        _economyProvider.CurrencySymbol,
                        _economyProvider.CurrencyName
                    }]);

            await _vehicleSpawner.SpawnVehicleAsync(user.Player, vehicleAsset.VehicleAssetId);

            return balance;
        }

        public async Task<bool> IsPermitted(IPermissionActor user)
        {
            var blacklistEnabled = _configuration.GetValue("shops:blacklistEnabled", false);
            var whitelistEnabled = _configuration.GetValue("shops:whitelistEnabled", false);

            if (!blacklistEnabled && !whitelistEnabled) return true;

            var groups = ShopData.AuthGroups.ToList();

            if (groups.Count == 0) return true;

            var blacklistedGroups = groups.Where(x => !x.IsWhitelist).ToList();
            var whitelistedGroups = groups.Where(x => x.IsWhitelist).ToList();

            if (whitelistEnabled && whitelistedGroups.Count > 0)
            {
                if (blacklistEnabled && blacklistedGroups.Count > 0)
                {
                    _logger.LogWarning(
                        $"Vehicle shop with id {ShopData.VehicleId} has both a whitelist and a blacklist. Defaulting to using whitelist.");
                }

                foreach (var group in whitelistedGroups)
                {
                    var permission = string.Format(PermissionFormat, group.Permission);

                    if (_permissionRegistry.FindPermission(_openModComponent, permission) == null)
                    {
                        _permissionRegistry.RegisterPermission(_openModComponent, permission,
                            description: "Provides/denies access to a shop.");
                    }

                    if (await _permissionChecker.CheckPermissionAsync(user, permission) == PermissionGrantResult.Grant)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (blacklistEnabled && blacklistedGroups.Count > 0)
            {
                foreach (var group in blacklistedGroups)
                {
                    var permission = string.Format(PermissionFormat, group.Permission);

                    if (_permissionRegistry.FindPermission(_openModComponent, permission) == null)
                    {
                        _permissionRegistry.RegisterPermission(_openModComponent, permission,
                            description: "Provides/denies access to a shop.");
                    }

                    if (await _permissionChecker.CheckPermissionAsync(user, permission) == PermissionGrantResult.Grant)
                    {
                        return false;
                    }
                }

                return true;
            }

            return true;
        }
    }
}
