using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class MessageInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    WorkingTimeFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkingTimeTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SegmentId = table.Column<int>(type: "int", nullable: true),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    MessageHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => new { x.ChatId, x.MessageHeaderId });
                    table.ForeignKey(
                        name: "FK_Messages_MessageHeaders_MessageHeaderId",
                        column: x => x.MessageHeaderId,
                        principalTable: "MessageHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageHeaderId",
                table: "Messages",
                column: "MessageHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "MessageHeaders");
        }
    }
}
