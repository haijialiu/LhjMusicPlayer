using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LhjMusicPlayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMusicFileUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_musics_FilePath",
                table: "musics",
                column: "FilePath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_musics_FilePath",
                table: "musics");
        }
    }
}
