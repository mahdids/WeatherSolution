using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class addLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CrawlWebPath_ForecastWindGFSLevel",
                table: "SystemSettings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CrawlWebPath_ForecastWindGFSLevelKeys",
                table: "SystemSettings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<byte>(
                name: "Level",
                table: "GfsForecasts",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrawlWebPath_ForecastWindGFSLevel",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "CrawlWebPath_ForecastWindGFSLevelKeys",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "GfsForecasts");
        }
    }
}
