using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class OnelastRewardModeladjustmenttoincludethetypethatisused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TriggeredType",
                table: "Rewards",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TriggeredType",
                table: "Rewards");
        }
    }
}
