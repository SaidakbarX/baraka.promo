using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class AddedPromoNewProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Promos",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCount",
                table: "Promos",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Promos");

            migrationBuilder.DropColumn(
                name: "TotalCount",
                table: "Promos");
        }
    }
}
