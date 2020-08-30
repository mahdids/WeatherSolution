using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class AddWindyTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ecmwfs_WindyTime_WindyTimeId",
                table: "Ecmwfs");

            migrationBuilder.DropForeignKey(
                name: "FK_Gfses_WindyTime_WindyTimeId",
                table: "Gfses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WindyTime",
                table: "WindyTime");

            migrationBuilder.RenameTable(
                name: "WindyTime",
                newName: "WindyTimes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WindyTimes",
                table: "WindyTimes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ecmwfs_WindyTimes_WindyTimeId",
                table: "Ecmwfs",
                column: "WindyTimeId",
                principalTable: "WindyTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gfses_WindyTimes_WindyTimeId",
                table: "Gfses",
                column: "WindyTimeId",
                principalTable: "WindyTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ecmwfs_WindyTimes_WindyTimeId",
                table: "Ecmwfs");

            migrationBuilder.DropForeignKey(
                name: "FK_Gfses_WindyTimes_WindyTimeId",
                table: "Gfses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WindyTimes",
                table: "WindyTimes");

            migrationBuilder.RenameTable(
                name: "WindyTimes",
                newName: "WindyTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WindyTime",
                table: "WindyTime",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ecmwfs_WindyTime_WindyTimeId",
                table: "Ecmwfs",
                column: "WindyTimeId",
                principalTable: "WindyTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gfses_WindyTime_WindyTimeId",
                table: "Gfses",
                column: "WindyTimeId",
                principalTable: "WindyTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
