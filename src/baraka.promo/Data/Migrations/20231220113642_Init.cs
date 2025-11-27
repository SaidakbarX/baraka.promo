using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromoId = table.Column<long>(type: "bigint", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeOfUse = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromoProducts",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PromoId = table.Column<long>(type: "bigint", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoProducts", x => new { x.ProductId, x.PromoId });
                });

            migrationBuilder.CreateTable(
                name: "PromoRegions",
                columns: table => new
                {
                    PromoId = table.Column<long>(type: "bigint", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoRegions", x => new { x.RegionId, x.PromoId });
                });

            migrationBuilder.CreateTable(
                name: "PromoRestaurants",
                columns: table => new
                {
                    PromoId = table.Column<long>(type: "bigint", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoRestaurants", x => new { x.RestaurantId, x.PromoId });
                });

            migrationBuilder.CreateTable(
                name: "Promos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinOrderAmount = table.Column<long>(type: "bigint", nullable: true),
                    MaxOrderAmount = table.Column<long>(type: "bigint", nullable: true),
                    OrderDiscount = table.Column<int>(type: "int", nullable: true),
                    View = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoClients");

            migrationBuilder.DropTable(
                name: "PromoProducts");

            migrationBuilder.DropTable(
                name: "PromoRegions");

            migrationBuilder.DropTable(
                name: "PromoRestaurants");

            migrationBuilder.DropTable(
                name: "Promos");
        }
    }
}
