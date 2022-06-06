using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Practice.Data.Migrations
{
    public partial class Addedchallengetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChallengeTypeId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ChallengeTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeTypes", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_ChallengeTypes_ChallengeTypeId",
                table: "Challenges");

            migrationBuilder.DropTable(
                name: "ChallengeTypes");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_ChallengeTypeId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "ChallengeTypeId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Challenges");
        }
    }
}
