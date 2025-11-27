using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baraka.promo.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageUz = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkingTimeFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkingTimeTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SegmentId = table.Column<int>(type: "int", nullable: true),
                    IsImage = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MessageRu = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MessageEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MessageKz = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    DeviceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Language = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => new { x.DeviceId, x.NotificationHeaderId });
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationHeaders_NotificationHeaderId",
                        column: x => x.NotificationHeaderId,
                        principalTable: "NotificationHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StopTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTimes", x => new { x.Id, x.NotificationHeaderId });
                    table.ForeignKey(
                        name: "FK_NotificationTimes_NotificationHeaders_NotificationHeaderId",
                        column: x => x.NotificationHeaderId,
                        principalTable: "NotificationHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationHeaderId",
                table: "Notifications",
                column: "NotificationHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTimes_NotificationHeaderId",
                table: "NotificationTimes",
                column: "NotificationHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTimes");

            migrationBuilder.DropTable(
                name: "NotificationHeaders");
        }
    }
}
