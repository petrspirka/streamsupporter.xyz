using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class Addedaudiotypetoalerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioType",
                table: "Alerts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioType",
                table: "Alerts");
        }
    }
}
