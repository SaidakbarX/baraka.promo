using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class MessageHeaderAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileId",
                table: "MessageHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImage",
                table: "MessageHeaders",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "MessageHeaders");

            migrationBuilder.DropColumn(
                name: "IsImage",
                table: "MessageHeaders");
        }
    }
}
