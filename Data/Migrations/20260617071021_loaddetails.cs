using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadManagementSystem_LMS_.Migrations
{
    /// <inheritdoc />
    public partial class loaddetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "Documents",
                newName: "FileType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "Documents",
                newName: "ContentType");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
