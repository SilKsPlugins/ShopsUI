using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore.Configurator;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using ShopsUI.Database.Models;
using System;

namespace ShopsUI.Database
{
    public class ShopsDbContext : OpenModDbContext<ShopsDbContext>
    {
        private readonly IServiceProvider _serviceProvider;

        public ShopsDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ShopsDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) : base(configurator, serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DbSet<ItemShopModel> ItemShops => Set<ItemShopModel>();

        public DbSet<VehicleShopModel> VehicleShops => Set<VehicleShopModel>();

        public DbSet<ItemGroupModel> ItemGroups => Set<ItemGroupModel>();

        public DbSet<VehicleGroupModel> VehicleGroups => Set<VehicleGroupModel>();

        public DbSet<ItemCategoryModel> ItemCategories => Set<ItemCategoryModel>();

        public DbSet<ItemShopCategoryModel> ItemShopCategories => Set<ItemShopCategoryModel>();

        public DbSet<VehicleCategoryModel> VehicleCategories => Set<VehicleCategoryModel>();

        public DbSet<VehicleShopCategoryModel> VehicleShopCategories => Set<VehicleShopCategoryModel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringName = GetConnectionStringName();
            var connectionStringAccessor = _serviceProvider.GetRequiredService<IConnectionStringAccessor>();
            var connectionString = connectionStringAccessor.GetConnectionString(connectionStringName);

            optionsBuilder.UseMySql(connectionString!,
                options =>
                {
                    options.MigrationsHistoryTable(MigrationsTableName);
                    options.ServerVersion(ServerVersion.AutoDetect(connectionString));
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemShopModel>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedNever(); 

                entity.HasMany(x => x.AuthGroups)
                    .WithOne(x => x.ItemShop)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Categories)
                    .WithOne(x => x.ItemShop)
                    .HasForeignKey(x => x.ItemShopId)
                    .IsRequired();
            });

            modelBuilder.Entity<VehicleShopModel>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedNever();

                entity.HasMany(x => x.AuthGroups)
                    .WithOne(x => x.VehicleShop)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Categories)
                    .WithOne(x => x.VehicleShop)
                    .HasForeignKey(x => x.VehicleShopId)
                    .IsRequired();
            });
            
            modelBuilder.Entity<ItemGroupModel>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<VehicleGroupModel>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ItemCategoryModel>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.HasMany(x => x.ItemShops)
                    .WithOne(x => x.ItemCategory)
                    .HasForeignKey(x => x.ItemCategoryId)
                    .IsRequired();
            });

            modelBuilder.Entity<ItemShopCategoryModel>()
                .HasKey(x => new {x.ItemCategoryId, x.ItemShopId});

            modelBuilder.Entity<VehicleCategoryModel>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.HasMany(x => x.VehicleShops)
                    .WithOne(x => x.VehicleCategory)
                    .HasForeignKey(x => x.VehicleCategoryId)
                    .IsRequired();
            });

            modelBuilder.Entity<VehicleShopCategoryModel>()
                .HasKey(x => new { x.VehicleCategoryId, x.VehicleShopId });
        }
    }
}
