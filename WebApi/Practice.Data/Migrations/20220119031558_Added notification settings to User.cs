using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Practice.Data.Migrations
{
    public partial class AddednotificationsettingstoUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NotificationsSettingsId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserNotificationsSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SendAchievementNotification = table.Column<bool>(type: "bit", nullable: false),
                    SendChallengeNotification = table.Column<bool>(type: "bit", nullable: false),
                    SendCommitNotification = table.Column<bool>(type: "bit", nullable: false),
                    SendUserChallengeNotification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationsSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NotificationsSettingsId",
                table: "AspNetUsers",
                column: "NotificationsSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserNotificationsSettings_NotificationsSettingsId",
                table: "AspNetUsers",
                column: "NotificationsSettingsId",
                principalTable: "UserNotificationsSettings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserNotificationsSettings_NotificationsSettingsId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserNotificationsSettings");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_NotificationsSettingsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotificationsSettingsId",
                table: "AspNetUsers");
        }
    }
}
