using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMovie.Migrations
{
    /// <inheritdoc />
    public partial class AddRecentlyWatched : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Season_SeasonId",
                table: "Episode");

            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Video_VideoId",
                table: "Episode");

            migrationBuilder.DropForeignKey(
                name: "FK_Movie_Video_VideoId",
                table: "Movie");

            migrationBuilder.DropForeignKey(
                name: "FK_Season_Show_ShowId",
                table: "Season");

            migrationBuilder.AddColumn<float>(
                name: "LastWatchedTimestamp",
                table: "Video",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "Season",
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

            migrationBuilder.AlterColumn<int>(
                name: "SeasonId",
                table: "Episode",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RecentlyWatched",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EpisodeId = table.Column<int>(type: "INTEGER", nullable: true),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: true),
                    WatchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentlyWatched", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentlyWatched_Episode_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecentlyWatched_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyWatched_EpisodeId",
                table: "RecentlyWatched",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyWatched_MovieId",
                table: "RecentlyWatched",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Episode_Season_SeasonId",
                table: "Episode",
                column: "SeasonId",
                principalTable: "Season",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Season_Show_ShowId",
                table: "Season",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Season_SeasonId",
                table: "Episode");

            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Video_VideoId",
                table: "Episode");

            migrationBuilder.DropForeignKey(
                name: "FK_Movie_Video_VideoId",
                table: "Movie");

            migrationBuilder.DropForeignKey(
                name: "FK_Season_Show_ShowId",
                table: "Season");

            migrationBuilder.DropTable(
                name: "RecentlyWatched");

            migrationBuilder.DropColumn(
                name: "LastWatchedTimestamp",
                table: "Video");

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "Season",
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

            migrationBuilder.AlterColumn<int>(
                name: "SeasonId",
                table: "Episode",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Episode_Season_SeasonId",
                table: "Episode",
                column: "SeasonId",
                principalTable: "Season",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Season_Show_ShowId",
                table: "Season",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "Id");
        }
    }
}
