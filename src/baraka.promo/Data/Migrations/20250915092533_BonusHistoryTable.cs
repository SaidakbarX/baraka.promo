using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class BonusHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusSum",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "BonusHistories",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashbackId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BonusUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashbackSum = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusHistories", x => new { x.TransactionId, x.CashbackId, x.CreatedAt });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusHistories");

            migrationBuilder.AddColumn<decimal>(
                name: "BonusSum",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
