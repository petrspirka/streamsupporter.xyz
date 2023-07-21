using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class AdjustedRewardModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewards_Alerts_TriggeredAlertId",
                table: "Rewards");

            migrationBuilder.DropForeignKey(
                name: "FK_Rewards_Marquees_TriggeredMarqueeId",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_TriggeredAlertId",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_TriggeredMarqueeId",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "TriggeredAlertId",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "TriggeredMarqueeId",
                table: "Rewards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TriggeredAlertId",
                table: "Rewards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TriggeredMarqueeId",
                table: "Rewards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_TriggeredAlertId",
                table: "Rewards",
                column: "TriggeredAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_TriggeredMarqueeId",
                table: "Rewards",
                column: "TriggeredMarqueeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewards_Alerts_TriggeredAlertId",
                table: "Rewards",
                column: "TriggeredAlertId",
                principalTable: "Alerts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewards_Marquees_TriggeredMarqueeId",
                table: "Rewards",
                column: "TriggeredMarqueeId",
                principalTable: "Marquees",
                principalColumn: "Id");
        }
    }
}
