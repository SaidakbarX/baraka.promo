using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class MessageTimeChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MessageTimes_MessageHeaderId",
                table: "MessageTimes",
                column: "MessageHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTimes_MessageHeaders_MessageHeaderId",
                table: "MessageTimes",
                column: "MessageHeaderId",
                principalTable: "MessageHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTimes_MessageHeaders_MessageHeaderId",
                table: "MessageTimes");

            migrationBuilder.DropIndex(
                name: "IX_MessageTimes_MessageHeaderId",
                table: "MessageTimes");
        }
    }
}
