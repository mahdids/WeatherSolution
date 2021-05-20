using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class systemSettingDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "SystemSettings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "SystemSettings",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SystemSettings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SystemSettings");
        }
    }
}
