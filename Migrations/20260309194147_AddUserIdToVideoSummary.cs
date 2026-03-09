using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SumaryYoutubeBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToVideoSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "VideoSummaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "VideoSummaries");
        }
    }
}
