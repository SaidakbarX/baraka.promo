using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class RebuildModelSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<long>(type: "bigint", nullable: false),
                    TimeOfUse = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionClients", x => new { x.Id, x.SubscriptionId });
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameUz = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    NameRu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    ShortContent_Ru = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_Uz = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_En = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Content_Ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_Uz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrlRu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrlUz = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrlEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ValidityPeriod = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NamePurchaseUz = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    NamePurchaseEn = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    NamePurchaseRu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ShortContent_Purchase_Ru = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_Purchase_Uz = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_Purchase_En = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Content_Purchase_Ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_Purchase_Uz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_Purchase_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl_Purchase_Ru = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl_Purchase_Uz = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl_Purchase_En = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameUz = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    NameRu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    ShortContent_Ru = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_Uz = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_En = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Content_Ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_Uz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrlRu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrlUz = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrlEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MXIK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PackageCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Vat = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NamePurchaseUz = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    NamePurchaseEn = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    NamePurchaseRu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ShortContent_Purchase_Ru = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_Purchase_Uz = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ShortContent_Purchase_En = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Content_Purchase_Ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_Purchase_Uz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content_Purchase_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl_Purchase_Ru = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl_Purchase_Uz = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl_Purchase_En = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PromotionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionClients");

            migrationBuilder.DropTable(
                name: "SubscriptionGroups");

            migrationBuilder.DropTable(
                name: "Subscriptions");
        }
    }
}
