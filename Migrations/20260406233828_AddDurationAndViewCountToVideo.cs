using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SumaryYoutubeBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationAndViewCountToVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "VideoSummaries",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<long>(
                name: "Vizualization",
                table: "VideoSummaries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "VideoSummaries");

            migrationBuilder.DropColumn(
                name: "Vizualization",
                table: "VideoSummaries");
        }
    }
}
