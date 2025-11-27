using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class AddSegmentchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmountTo",
                table: "Segments",
                newName: "TotalAmountMin");

            migrationBuilder.RenameColumn(
                name: "TotalAmountFrom",
                table: "Segments",
                newName: "TotalAmountMax");

            migrationBuilder.RenameColumn(
                name: "QuantityTo",
                table: "Segments",
                newName: "QuantityMin");

            migrationBuilder.RenameColumn(
                name: "QuantityFrom",
                table: "Segments",
                newName: "QuantityMax");

            migrationBuilder.RenameColumn(
                name: "AmountTo",
                table: "Segments",
                newName: "AmountMin");

            migrationBuilder.RenameColumn(
                name: "AmountFrom",
                table: "Segments",
                newName: "AmountMax");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmountMin",
                table: "Segments",
                newName: "TotalAmountTo");

            migrationBuilder.RenameColumn(
                name: "TotalAmountMax",
                table: "Segments",
                newName: "TotalAmountFrom");

            migrationBuilder.RenameColumn(
                name: "QuantityMin",
                table: "Segments",
                newName: "QuantityTo");

            migrationBuilder.RenameColumn(
                name: "QuantityMax",
                table: "Segments",
                newName: "QuantityFrom");

            migrationBuilder.RenameColumn(
                name: "AmountMin",
                table: "Segments",
                newName: "AmountTo");

            migrationBuilder.RenameColumn(
                name: "AmountMax",
                table: "Segments",
                newName: "AmountFrom");
        }
    }
}
