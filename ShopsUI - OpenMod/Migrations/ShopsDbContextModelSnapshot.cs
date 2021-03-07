﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShopsUI.Database;

namespace ShopsUI.Migrations
{
    [DbContext(typeof(ShopsDbContext))]
    partial class ShopsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ShopsUI.Database.Models.ItemShopModel", b =>
                {
                    b.Property<int>("ItemShopId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal?>("BuyPrice")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<decimal?>("SellPrice")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("ItemShopId");

                    b.ToTable("ShopsUI_ItemShops");
                });

            modelBuilder.Entity("ShopsUI.Database.Models.VehicleShopModel", b =>
                {
                    b.Property<int>("VehicleShopId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("BuyPrice")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("VehicleShopId");

                    b.ToTable("ShopsUI_VehicleShops");
                });
#pragma warning restore 612, 618
        }
    }
}
