extern alias MySqlConnectorAnnotations;
using Cysharp.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySqlConnectorAnnotations::MySql.Data.MySqlClient;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using SDG.Unturned;
using ShopsUI.API.Migrations;
using ShopsUI.Commands.Items;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace ShopsUI.Commands
{
    [Command("migrate", Priority = Priority.High)]
    [CommandDescription("Migrates ZaupShop shops to ShopsUI.")]
    [CommandParent(typeof(CShop))]
    public class CShopMigrate : UnturnedCommand
    {
        // /Rocket/Plugins/Uconomy/Uconomy.configuration.xml
        [Serializable]
        public class UconomyConfiguration
        {
            public string? DatabaseAddress { get; set; }

            public string? DatabaseUsername { get; set; }

            public string? DatabasePassword { get; set; }

            public string? DatabaseName { get; set; }

            public int DatabasePort { get; set; }
        }

        // /Rocket/Plugins/ZaupShop/ZaupShop.configuration.xml
        [Serializable]
        public class ZaupShopConfiguration
        {
            public string ItemShopTableName { get; set; } = "uconomyitemshop";

            public string VehicleShopTableName { get; set; } = "uconomyvehicleshop";

            public string? GroupListTableName { get; set; } = null;
        }

        private readonly ShopsDbContext _dbContext;
        private readonly IMigrationVerifier _migrationVerifier;

        public CShopMigrate(ShopsDbContext dbContext,
            IMigrationVerifier migrationVerifier,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = dbContext;
            _migrationVerifier = migrationVerifier;
        }

        private async Task<T> GetPluginConfig<T>(string path) where T : class, new()
        {
            var fullPath = ReadWrite.PATH + ServerSavedata.transformPath(path);

            if (!File.Exists(fullPath))
            {
                throw new UserFriendlyException(
                    $"Could not find a plugin configuration file: {Path.GetFileName(path)}");
            }
            
            using var streamReader = new StreamReader(fullPath);

            var contents = await streamReader.ReadToEndAsync();

            var reader = new StringReader(contents);

            var serializer = new XmlSerializer(typeof(T));

            try
            {
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception)
            {
                throw new UserFriendlyException(
                    $"There was an issue parsing the configuration file: {Path.GetFileName(path)}");
            }
        }

        private async Task ClearExistingDatabase()
        {
            async Task ClearSet<T>(DbSet<T> set) where T : class
            {
                set.RemoveRange(await set.ToListAsync());
            }

            await ClearSet(_dbContext.ItemGroups);
            await ClearSet(_dbContext.VehicleGroups);
            await ClearSet(_dbContext.ItemShops);
            await ClearSet(_dbContext.VehicleShops);
        }

        private async Task MigrateItemShops(MySqlConnection connection, ZaupShopConfiguration config)
        {
            var query = $"SELECT `id`,`cost`,`buyback` FROM `{config.ItemShopTableName}`;";

            var command = new MySqlCommand(query, connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var id = reader.GetUInt16(0);
                var buyPrice = reader.GetDecimal(1);
                var sellPrice = reader.GetDecimal(2);

                await _dbContext.ItemShops.AddAsync(new ItemShopModel
                {
                    ItemId = id,
                    BuyPrice = buyPrice <= 0 ? null : buyPrice,
                    SellPrice = sellPrice <= 0 ? null : sellPrice,
                    Order = 0
                });
            }
        }

        private async Task MigrateVehicleShops(MySqlConnection connection, ZaupShopConfiguration config)
        {
            var query = $"SELECT `id`,`cost` FROM `{config.VehicleShopTableName}`;";

            var command = new MySqlCommand(query, connection);

            using var reader = command.ExecuteReader();

            while (await reader.ReadAsync())
            {
                var id = reader.GetUInt16(0);
                var buyPrice = reader.GetDecimal(1);

                await _dbContext.VehicleShops.AddAsync(new VehicleShopModel
                {
                    VehicleId = id,
                    BuyPrice = buyPrice,
                    Order = 0
                });
            }
        }

        private async Task MigrateGroups(MySqlConnection connection, ZaupShopConfiguration config)
        {
            var mainGroupQuery = $"SELECT `name`,`whitelist` FROM `{config.GroupListTableName}`;";

            var tables = new List<(string Name, bool Whitelist)>();

            await using (var command = new MySqlCommand(mainGroupQuery, connection))
            {
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var name = reader.GetString(0);
                    var whitelist = reader.GetByte(1) == 1; // 1 = whitelist, 0 = blacklist

                    tables.Add((name, whitelist));
                }
            }


            foreach (var (name, whitelist) in tables)
            {
                var groupTableQuery = $"SELECT `assetid`,`vehicle` FROM `{name}`;";

                await using var command = new MySqlCommand(groupTableQuery, connection);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var id = reader.GetUInt16(0);
                    var isVehicle = reader.GetByte(1) == 1; // 1 = vehicle, 0 = item

                    if (isVehicle)
                    {
                        await _dbContext.VehicleGroups.AddAsync(new VehicleGroupModel
                        {
                            Permission = name,
                            IsWhitelist = whitelist,
                            VehicleShopVehicleId = id
                        });
                    }
                    else
                    {
                        await _dbContext.ItemGroups.AddAsync(new ItemGroupModel
                        {
                            Permission = name,
                            IsWhitelist = whitelist,
                            ItemShopItemId = id
                        });
                    }
                }
            }
        }

        protected override async UniTask OnExecuteAsync()
        {
            if (!_migrationVerifier.CheckShouldContinueMigration(Context.Actor))
            {
                await PrintAsync("By using this migration command, you will erase all existing ShopsUI shops. " +
                                 "If you wish to continue, please run the command again.");

                return;
            }

            await UniTask.SwitchToThreadPool();

            await PrintAsync("Beginning shop migration. This may take a while.");

            var uconomyConfig =
                await GetPluginConfig<UconomyConfiguration>("/Rocket/Plugins/Uconomy/Uconomy.configuration.xml");
            var zaupShopConfig =
                await GetPluginConfig<ZaupShopConfiguration>("/Rocket/Plugins/ZaupShop/ZaupShop.configuration.xml");

            var migrateGroups = !string.IsNullOrEmpty(zaupShopConfig.GroupListTableName);

            var connectionString =
                $"Server={uconomyConfig.DatabaseAddress};" +
                $"Port={(uconomyConfig.DatabasePort == 0 ? 3306 : uconomyConfig.DatabasePort)};" +
                $"Database={uconomyConfig.DatabaseName};" +
                $"User={uconomyConfig.DatabaseUsername};" +
                $"Password={uconomyConfig.DatabasePassword}";

            await ClearExistingDatabase();

            await using var connection = new MySqlConnection(connectionString);

            await connection.OpenAsync();

            await MigrateItemShops(connection, zaupShopConfig);

            await MigrateVehicleShops(connection, zaupShopConfig);

            if (migrateGroups)
            {
                await MigrateGroups(connection, zaupShopConfig);
            }

            await connection.CloseAsync();

            await _dbContext.SaveChangesAsync();

            var itemShops = await _dbContext.ItemShops.CountAsync();
            var vehicleShops = await _dbContext.VehicleShops.CountAsync();

            await PrintAsync($"Successfully migrated {itemShops} item shops and {vehicleShops} vehicle shops.");

            if (migrateGroups)
            {
                await PrintAsync("Make sure to replace your zaupgroup.XXX permissions with ShopsUI:groups.XXX");
            }
        }
    }
}
