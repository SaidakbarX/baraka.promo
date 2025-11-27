using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class PromoChildsKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PromoChildValues",
                table: "PromoChildValues");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PromoChildValues",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "PromoChildValues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromoChildValues",
                table: "PromoChildValues",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PromoChildValues",
                table: "PromoChildValues");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PromoChildValues");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PromoChildValues",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromoChildValues",
                table: "PromoChildValues",
                columns: new[] { "Name", "PromoId" });
        }
    }
}
