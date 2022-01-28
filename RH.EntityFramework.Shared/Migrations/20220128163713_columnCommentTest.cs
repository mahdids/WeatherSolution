using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class columnCommentTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "Kelvin",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "Kelvin",
                oldClrType: typeof(double),
                oldType: "double");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Kelvin");

            migrationBuilder.AlterColumn<double>(
                name: "Temperature",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Kelvin");
        }
    }
}
