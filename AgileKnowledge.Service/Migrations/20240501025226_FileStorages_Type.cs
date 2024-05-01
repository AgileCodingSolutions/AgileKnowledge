using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgileKnowledge.Service.Migrations
{
    /// <inheritdoc />
    public partial class FileStorages_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FileStorages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "FileStorages");
        }
    }
}
