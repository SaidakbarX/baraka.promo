using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class PromoProductFeatureConditionProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoConditionType",
                table: "PromoProducts");

            migrationBuilder.CreateTable(
                name: "PromoProductFeatures",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PromoId = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoProductFeatures", x => new { x.ProductId, x.PromoId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoProductFeatures");

            migrationBuilder.AddColumn<int>(
                name: "PromoConditionType",
                table: "PromoProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
