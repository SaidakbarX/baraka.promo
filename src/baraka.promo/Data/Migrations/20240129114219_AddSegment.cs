using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class AddSegment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Segments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderTimeFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderTimeTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantityFrom = table.Column<int>(type: "int", nullable: true),
                    QuantityTo = table.Column<int>(type: "int", nullable: true),
                    AmountFrom = table.Column<long>(type: "bigint", nullable: true),
                    AmountTo = table.Column<long>(type: "bigint", nullable: true),
                    TotalAmountFrom = table.Column<long>(type: "bigint", nullable: true),
                    TotalAmountTo = table.Column<long>(type: "bigint", nullable: true),
                    OrderTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Segments");
        }
    }
}
