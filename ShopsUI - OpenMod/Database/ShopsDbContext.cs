using Microsoft.EntityFrameworkCore;
using ShopsUI.Database.Models;
using SilK.OpenMod.EntityFrameworkCore;
using System;

namespace ShopsUI.Database
{
    public class ShopsDbContext : OpenModPomeloDbContext<ShopsDbContext>
    {
        public ShopsDbContext(
            DbContextOptions<ShopsDbContext> options,
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        public DbSet<ItemShopModel> ItemShops => Set<ItemShopModel>();

        public DbSet<VehicleShopModel> VehicleShops => Set<VehicleShopModel>();

        public DbSet<ItemGroupModel> ItemGroups => Set<ItemGroupModel>();

        public DbSet<VehicleGroupModel> VehicleGroups => Set<VehicleGroupModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemShopModel>()
                .HasKey(x => x.ItemId);

            modelBuilder.Entity<ItemShopModel>()
                .Property(x => x.ItemId)
                .ValueGeneratedNever();

            modelBuilder.Entity<ItemShopModel>()
                .HasMany(x => x.AuthGroups)
                .WithOne(x => x.ItemShop)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VehicleShopModel>()
                .HasKey(x => x.VehicleId);

            modelBuilder.Entity<VehicleShopModel>()
                .Property(x => x.VehicleId)
                .ValueGeneratedNever();

            modelBuilder.Entity<VehicleShopModel>()
                .HasMany(x => x.AuthGroups)
                .WithOne(x => x.VehicleShop)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemGroupModel>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<VehicleGroupModel>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
