using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class PomeloMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "ShopsUI_VehicleShops",
                type: "decimal(24,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "ShopsUI_ItemShops",
                type: "decimal(24,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "ShopsUI_ItemShops",
                type: "decimal(24,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "ShopsUI_VehicleShops",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(24,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "ShopsUI_ItemShops",
                type: "decimal(18, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(24,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "ShopsUI_ItemShops",
                type: "decimal(18, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(24,2)",
                oldNullable: true);
        }
    }
}
