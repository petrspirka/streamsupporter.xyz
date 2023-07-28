using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewStreamSupporter.Migrations
{
    /// <inheritdoc />
    public partial class Adjusteddatatypesofwidgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SpeedFactorPerCharacter",
                table: "Marquees",
                type: "decimal(4, 2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "SpeedFactor",
                table: "Marquees",
                type: "decimal(4, 2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "FadeTime",
                table: "Marquees",
                type: "decimal(4, 2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<float>(
                name: "Delay",
                table: "Marquees",
                type: "decimal(7, 2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "AlertDuration",
                table: "Alerts",
                type: "decimal(9, 2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "SpeedFactorPerCharacter",
                table: "Marquees",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4, 2)");

            migrationBuilder.AlterColumn<float>(
                name: "SpeedFactor",
                table: "Marquees",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4, 2)");

            migrationBuilder.AlterColumn<float>(
                name: "FadeTime",
                table: "Marquees",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4, 2)");

            migrationBuilder.AlterColumn<float>(
                name: "Delay",
                table: "Marquees",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "decimal(7, 2)");

            migrationBuilder.AlterColumn<float>(
                name: "AlertDuration",
                table: "Alerts",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9, 2)");
        }
    }
}
