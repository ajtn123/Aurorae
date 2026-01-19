using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aurorae.Migrations
{
    /// <inheritdoc />
    public partial class AddPixivIllustInfoError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "PixivIllustInfos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                table: "PixivIllustInfos");
        }
    }
}
