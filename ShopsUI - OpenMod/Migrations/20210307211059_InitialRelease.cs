using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class InitialRelease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopsUI_ItemShops",
                columns: table => new
                {
                    ItemId = table.Column<ushort>(nullable: false),
                    BuyPrice = table.Column<decimal>(type: "decimal(24,2)", nullable: true),
                    SellPrice = table.Column<decimal>(type: "decimal(24,2)", nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_ItemShops", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_VehicleShops",
                columns: table => new
                {
                    VehicleId = table.Column<ushort>(nullable: false),
                    BuyPrice = table.Column<decimal>(type: "decimal(24,2)", nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_VehicleShops", x => x.VehicleId);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_ItemGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Permission = table.Column<string>(nullable: false),
                    IsWhitelist = table.Column<bool>(nullable: false),
                    ItemShopItemId = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_ItemGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                        column: x => x.ItemShopItemId,
                        principalTable: "ShopsUI_ItemShops",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_VehicleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Permission = table.Column<string>(nullable: false),
                    IsWhitelist = table.Column<bool>(nullable: false),
                    VehicleShopVehicleId = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_VehicleGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                        column: x => x.VehicleShopVehicleId,
                        principalTable: "ShopsUI_VehicleShops",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_ItemGroups_ItemShopItemId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_VehicleGroups_VehicleShopVehicleId",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopVehicleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopsUI_ItemGroups");

            migrationBuilder.DropTable(
                name: "ShopsUI_VehicleGroups");

            migrationBuilder.DropTable(
                name: "ShopsUI_ItemShops");

            migrationBuilder.DropTable(
                name: "ShopsUI_VehicleShops");
        }
    }
}
