using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class EnsureNoGeneratedShopIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
