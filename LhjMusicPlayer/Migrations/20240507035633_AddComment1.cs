using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LhjMusicPlayer.Migrations
{
    /// <inheritdoc />
    public partial class AddComment1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "Datetime(CURRENT_TIMESTAMP,'localtime')"),
                    MusicId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentList_musics_MusicId",
                        column: x => x.MusicId,
                        principalTable: "musics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "评论");

            migrationBuilder.CreateIndex(
                name: "IX_CommentList_MusicId",
                table: "CommentList",
                column: "MusicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentList");
        }
    }
}
