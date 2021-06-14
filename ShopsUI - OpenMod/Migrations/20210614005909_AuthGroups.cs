using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class AuthGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
