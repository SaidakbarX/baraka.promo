using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class CardInfos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LoyalityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "HolderId",
                table: "Cards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "HolderName",
                table: "Cards",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductInfo",
                table: "Cards",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LoyalityTypes");

            migrationBuilder.DropColumn(
                name: "HolderId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "HolderName",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ProductInfo",
                table: "Cards");
        }
    }
}
