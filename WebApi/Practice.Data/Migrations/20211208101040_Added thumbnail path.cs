using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Practice.Data.Migrations
{
    public partial class Addedthumbnailpath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_ChallengeTypes_ChallengeTypeId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_ChallengeTypeId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "ChallengeTypeId",
                table: "Challenges");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "ChallengeImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_TypeId",
                table: "Challenges",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_ChallengeTypes_TypeId",
                table: "Challenges",
                column: "TypeId",
                principalTable: "ChallengeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_ChallengeTypes_TypeId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_TypeId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "ChallengeImages");

            migrationBuilder.AddColumn<Guid>(
                name: "ChallengeTypeId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengeTypeId",
                table: "Challenges",
                column: "ChallengeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_ChallengeTypes_ChallengeTypeId",
                table: "Challenges",
                column: "ChallengeTypeId",
                principalTable: "ChallengeTypes",
                principalColumn: "Id");
        }
    }
}
