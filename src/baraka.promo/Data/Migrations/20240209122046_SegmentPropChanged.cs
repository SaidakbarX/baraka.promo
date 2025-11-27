using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class SegmentPropChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Segment",
                table: "Promos");

            migrationBuilder.AddColumn<int>(
                name: "SegmentId",
                table: "Promos",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SegmentId",
                table: "Promos");

            migrationBuilder.AddColumn<string>(
                name: "Segment",
                table: "Promos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
