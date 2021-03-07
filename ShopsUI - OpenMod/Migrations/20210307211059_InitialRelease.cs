using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

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
                    ItemShopId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BuyPrice = table.Column<decimal>(nullable: true),
                    SellPrice = table.Column<decimal>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_ItemShops", x => x.ItemShopId);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_VehicleShops",
                columns: table => new
                {
                    VehicleShopId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BuyPrice = table.Column<decimal>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_VehicleShops", x => x.VehicleShopId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopsUI_ItemShops");

            migrationBuilder.DropTable(
                name: "ShopsUI_VehicleShops");
        }
    }
}
