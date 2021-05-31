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

        public DbSet<ItemShopModel> ItemShops { get; set; } = null!;

        public DbSet<VehicleShopModel> VehicleShops { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemShopModel>()
                .HasKey(x => x.ItemId);

            modelBuilder.Entity<VehicleShopModel>()
                .HasKey(x => x.VehicleId);
        }
    }
}
