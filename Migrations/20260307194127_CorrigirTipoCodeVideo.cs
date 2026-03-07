using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SumaryYoutubeBackend.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirTipoCodeVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodeVideoId",
                table: "VideoSummaries",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CodeVideoId",
                table: "VideoSummaries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
