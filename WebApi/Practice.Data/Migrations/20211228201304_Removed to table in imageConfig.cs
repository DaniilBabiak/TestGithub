using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practice.Data.Migrations
{
    public partial class RemovedtotableinimageConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Challenges_ChallengeId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ChallengeId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ChallengeId",
                table: "Images",
                newName: "EntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Images",
                newName: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ChallengeId",
                table: "Images",
                column: "ChallengeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Challenges_ChallengeId",
                table: "Images",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
