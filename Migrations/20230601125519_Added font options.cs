using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class Addedfontoptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Purchases");

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "TimerModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "TimerModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "Rewards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "Rewards",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "Marquees",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "Marquees",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "DonationGoalModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "DonationGoalModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "CounterModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "CounterModel",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "FontFamily",
                table: "Alerts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "FontSize",
                table: "Alerts",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "TimerModel");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "TimerModel");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "Marquees");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "Marquees");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "DonationGoalModel");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "DonationGoalModel");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "CounterModel");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "CounterModel");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "Alerts");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Purchases",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }
    }
}
