using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopsUI.Migrations
{
    public partial class ShopsRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                table: "ShopsUI_ItemGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops");

            migrationBuilder.DropIndex(
                name: "IX_ShopsUI_VehicleGroups_VehicleShopVehicleId",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops");

            migrationBuilder.DropIndex(
                name: "IX_ShopsUI_ItemGroups_ItemShopItemId",
                table: "ShopsUI_ItemGroups");
            
            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "ShopsUI_VehicleShops",
                newName: "Id");
            
            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ShopsUI_ItemShops",
                newName: "Id");

            migrationBuilder.AddColumn<ushort>(
                name: "VehicleShopId",
                table: "ShopsUI_VehicleGroups",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "ItemShopId",
                table: "ShopsUI_ItemGroups",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ShopsUI_ItemCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_ItemCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_VehicleCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_VehicleCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_ItemShopCategories",
                columns: table => new
                {
                    ItemShopId = table.Column<ushort>(nullable: false),
                    ItemCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_ItemShopCategories", x => new { x.ItemCategoryId, x.ItemShopId });
                    table.ForeignKey(
                        name: "FK_ShopsUI_ItemShopCategories_ShopsUI_ItemCategories_ItemCatego~",
                        column: x => x.ItemCategoryId,
                        principalTable: "ShopsUI_ItemCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopsUI_ItemShopCategories_ShopsUI_ItemShops_ItemShopId",
                        column: x => x.ItemShopId,
                        principalTable: "ShopsUI_ItemShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopsUI_VehicleShopCategories",
                columns: table => new
                {
                    VehicleShopId = table.Column<ushort>(nullable: false),
                    VehicleCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopsUI_VehicleShopCategories", x => new { x.VehicleCategoryId, x.VehicleShopId });
                    table.ForeignKey(
                        name: "FK_ShopsUI_VehicleShopCategories_ShopsUI_VehicleCategories_Vehi~",
                        column: x => x.VehicleCategoryId,
                        principalTable: "ShopsUI_VehicleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopsUI_VehicleShopCategories_ShopsUI_VehicleShops_VehicleSh~",
                        column: x => x.VehicleShopId,
                        principalTable: "ShopsUI_VehicleShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_VehicleGroups_VehicleShopId",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_ItemGroups_ItemShopId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_ItemShopCategories_ItemShopId",
                table: "ShopsUI_ItemShopCategories",
                column: "ItemShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_VehicleShopCategories_VehicleShopId",
                table: "ShopsUI_VehicleShopCategories",
                column: "VehicleShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopId",
                principalTable: "ShopsUI_ItemShops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopId",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopId",
                principalTable: "ShopsUI_VehicleShops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopId",
                table: "ShopsUI_ItemGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopId",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropTable(
                name: "ShopsUI_ItemShopCategories");

            migrationBuilder.DropTable(
                name: "ShopsUI_VehicleShopCategories");

            migrationBuilder.DropTable(
                name: "ShopsUI_ItemCategories");

            migrationBuilder.DropTable(
                name: "ShopsUI_VehicleCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops");

            migrationBuilder.DropIndex(
                name: "IX_ShopsUI_VehicleGroups_VehicleShopId",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops");

            migrationBuilder.DropIndex(
                name: "IX_ShopsUI_ItemGroups_ItemShopId",
                table: "ShopsUI_ItemGroups");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShopsUI_VehicleShops",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShopsUI_ItemShops",
                newName: "ItemId");

            migrationBuilder.DropColumn(
                name: "VehicleShopId",
                table: "ShopsUI_VehicleGroups");

            migrationBuilder.DropColumn(
                name: "ItemShopId",
                table: "ShopsUI_ItemGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_VehicleShops",
                table: "ShopsUI_VehicleShops",
                column: "VehicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopsUI_ItemShops",
                table: "ShopsUI_ItemShops",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_VehicleGroups_VehicleShopVehicleId",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopsUI_ItemGroups_ItemShopItemId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_ItemGroups_ShopsUI_ItemShops_ItemShopItemId",
                table: "ShopsUI_ItemGroups",
                column: "ItemShopItemId",
                principalTable: "ShopsUI_ItemShops",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopsUI_VehicleGroups_ShopsUI_VehicleShops_VehicleShopVehicl~",
                table: "ShopsUI_VehicleGroups",
                column: "VehicleShopVehicleId",
                principalTable: "ShopsUI_VehicleShops",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
