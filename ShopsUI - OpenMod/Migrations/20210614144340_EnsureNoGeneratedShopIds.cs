using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class EnsureNoGeneratedShopIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                table: "ShopsUI_ItemGroups");

            migrationBuilder.AlterColumn<ushort>(
                name: "VehicleId",
                table: "ShopsUI_VehicleShops",
                nullable: false,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ushort>(
                name: "ItemId",
                table: "ShopsUI_ItemShops",
                nullable: false,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopVehicleId",
                principalTable: "ShopsUI_VehicleShops",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopItemId",
                principalTable: "ShopsUI_ItemShops",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                table: "ShopsUI_ItemGroups");

            migrationBuilder.AlterColumn<ushort>(
                name: "VehicleId",
                table: "ShopsUI_VehicleShops",
                type: "smallint unsigned",
                nullable: false,
                oldClrType: typeof(ushort))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<ushort>(
                name: "ItemId",
                table: "ShopsUI_ItemShops",
                type: "smallint unsigned",
                nullable: false,
                oldClrType: typeof(ushort))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopVehicleId",
                principalTable: "ShopsUI_VehicleShops",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopItemId",
                principalTable: "ShopsUI_ItemShops",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
