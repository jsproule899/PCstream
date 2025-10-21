using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMovie.Migrations
{
    /// <inheritdoc />
    public partial class MovieToVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "videoId",
                table: "Movie",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Movie_videoId",
                table: "Movie",
                column: "videoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_Video_videoId",
                table: "Movie",
                column: "videoId",
                principalTable: "Video",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_Video_videoId",
                table: "Movie");

            migrationBuilder.DropIndex(
                name: "IX_Movie_videoId",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "videoId",
                table: "Movie");
        }
    }
}
