using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class MessageHeaderStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MessageHeaders");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MessageHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageHeaders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MessageHeaders");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MessageHeaders",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
