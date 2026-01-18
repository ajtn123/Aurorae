using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aurorae.Migrations
{
    /// <inheritdoc />
    public partial class AddFileMetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileMetas",
                columns: table => new
                {
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    Favorite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetas", x => x.FilePath);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMetas");
        }
    }
}
