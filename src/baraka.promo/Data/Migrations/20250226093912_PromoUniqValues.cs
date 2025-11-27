using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class PromoUniqValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUnique",
                table: "Promos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PromoChildValues",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PromoId = table.Column<long>(type: "bigint", nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TimeOfUse = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoChildValues", x => new { x.Name, x.PromoId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoChildValues");

            migrationBuilder.DropColumn(
                name: "IsUnique",
                table: "Promos");
        }
    }
}
