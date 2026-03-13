using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SumaryYoutubeBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldAuthUserIdFromVideoSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoSummaries_AuthUsers_AuthUserId",
                table: "VideoSummaries");

            migrationBuilder.DropIndex(
                name: "IX_VideoSummaries_AuthUserId",
                table: "VideoSummaries");

            migrationBuilder.DropColumn(
                name: "AuthUserId",
                table: "VideoSummaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthUserId",
                table: "VideoSummaries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoSummaries_AuthUserId",
                table: "VideoSummaries",
                column: "AuthUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoSummaries_AuthUsers_AuthUserId",
                table: "VideoSummaries",
                column: "AuthUserId",
                principalTable: "AuthUsers",
                principalColumn: "Id");
        }
    }
}
