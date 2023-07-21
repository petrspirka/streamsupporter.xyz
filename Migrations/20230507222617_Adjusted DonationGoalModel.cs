using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class AdjustedDonationGoalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimationLength",
                table: "DonationGoalModel");

            migrationBuilder.DropColumn(
                name: "DoesTextHaveShadow",
                table: "DonationGoalModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AnimationLength",
                table: "DonationGoalModel",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "DoesTextHaveShadow",
                table: "DonationGoalModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
