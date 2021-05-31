using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class RenamedPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops");

            migrationBuilder.RenameColumn(
                name: "VehicleShopId",
                table: "ShopsUI_VehicleShops",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "ItemShopId",
                table: "ShopsUI_ItemShops",
                newName: "ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops",
                column: "VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "ShopsUI_VehicleShops",
                newName: "VehicleShopId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ShopsUI_ItemShops",
                newName: "ItemShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops",
                column: "VehicleShopId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops",
                column: "ItemShopId");
        }
    }
}
