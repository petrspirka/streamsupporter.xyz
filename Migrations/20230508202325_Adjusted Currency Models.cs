using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class AdjustedCurrencyModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "UnclaimedCurrency");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ClaimedCurrency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "UnclaimedCurrency",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "ClaimedCurrency",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
