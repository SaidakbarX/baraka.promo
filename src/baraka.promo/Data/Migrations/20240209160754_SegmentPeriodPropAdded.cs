using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class SegmentPeriodPropAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderPeriodFrom",
                table: "Segments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderPeriodTo",
                table: "Segments",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPeriodFrom",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "OrderPeriodTo",
                table: "Segments");
        }
    }
}
