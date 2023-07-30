using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class Addeddatabaserestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClaimedCurrency_ShopOwnerId",
                table: "ClaimedCurrency");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ClaimedCurrency_ShopOwnerId_OwnerId",
                table: "ClaimedCurrency",
                columns: new[] { "ShopOwnerId", "OwnerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ClaimedCurrency_ShopOwnerId_OwnerId",
                table: "ClaimedCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimedCurrency_ShopOwnerId",
                table: "ClaimedCurrency",
                column: "ShopOwnerId");
        }
    }
}
