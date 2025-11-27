using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class SegmentNewPropsAddedInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderTypeId",
                table: "Segments");

            migrationBuilder.AddColumn<bool>(
                name: "IsNewClient",
                table: "Segments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LogicalOperator",
                table: "Segments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrderTypeIds",
                table: "Segments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantIds",
                table: "Segments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNewClient",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "LogicalOperator",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "OrderTypeIds",
                table: "Segments");

            migrationBuilder.DropColumn(
                name: "RestaurantIds",
                table: "Segments");

            migrationBuilder.AddColumn<int>(
                name: "OrderTypeId",
                table: "Segments",
                type: "int",
                nullable: true);
        }
    }
}
