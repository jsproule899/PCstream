using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMovie.Migrations
{
    /// <inheritdoc />
    public partial class NavigationalSetters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Video_VideoId",
                table: "Episode");

            migrationBuilder.DropForeignKey(
                name: "FK_Movie_Video_VideoId",
                table: "Movie");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfSeasons",
                table: "Show",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "VideoId",
                table: "Movie",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "VideoId",
                table: "Episode",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Episode_Video_VideoId",
                table: "Episode",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_Video_VideoId",
                table: "Movie",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Video_VideoId",
                table: "Episode");

            migrationBuilder.DropForeignKey(
                name: "FK_Movie_Video_VideoId",
                table: "Movie");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfSeasons",
                table: "Show",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VideoId",
                table: "Movie",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VideoId",
                table: "Episode",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Episode_Video_VideoId",
                table: "Episode",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_Video_VideoId",
                table: "Movie",
                column: "VideoId",
                principalTable: "Video",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
