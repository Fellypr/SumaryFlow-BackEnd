using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SumaryYoutubeBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthUserId",
                table: "VideoSummaries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoSummaries_AuthUsers_AuthUserId",
                table: "VideoSummaries");

            migrationBuilder.DropTable(
                name: "AuthUsers");

            migrationBuilder.DropIndex(
                name: "IX_VideoSummaries_AuthUserId",
                table: "VideoSummaries");

            migrationBuilder.DropColumn(
                name: "AuthUserId",
                table: "VideoSummaries");
        }
    }
}
