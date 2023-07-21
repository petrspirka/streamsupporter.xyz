using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUnclaimedCurrencyModeltoincludeconcurrencycheckandaddeddefaultNamevalueasitsunused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "UnclaimedCurrency");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ClaimedCurrency");

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "UnclaimedCurrency",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "UnclaimedCurrency");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UnclaimedCurrency",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ClaimedCurrency",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }
    }
}
