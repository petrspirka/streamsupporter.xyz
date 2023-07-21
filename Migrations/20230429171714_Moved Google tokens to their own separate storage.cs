using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class MovedGoogletokenstotheirownseparatestorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleAccessToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GoogleAccessTokenExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GoogleRefreshToken",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleActive",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTwitchActive",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoogleActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsTwitchActive",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "GoogleAccessToken",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoogleAccessTokenExpiry",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleRefreshToken",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }
    }
}
