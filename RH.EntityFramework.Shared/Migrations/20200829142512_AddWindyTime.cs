using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class AddWindyTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Start",
                table: "Gfses");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "Gfses");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Ecmwfs");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "Ecmwfs");

            migrationBuilder.AddColumn<int>(
                name: "WindyTimeId",
                table: "Gfses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WindyTimeId",
                table: "Ecmwfs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WindyTime",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: true),
                    Start = table.Column<long>(nullable: false),
                    Step = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindyTime", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gfses_WindyTimeId",
                table: "Gfses",
                column: "WindyTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ecmwfs_WindyTimeId",
                table: "Ecmwfs",
                column: "WindyTimeId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ecmwfs_WindyTime_WindyTimeId",
                table: "Ecmwfs");

            migrationBuilder.DropForeignKey(
                name: "FK_Gfses_WindyTime_WindyTimeId",
                table: "Gfses");

            migrationBuilder.DropTable(
                name: "WindyTime");

            migrationBuilder.DropIndex(
                name: "IX_Gfses_WindyTimeId",
                table: "Gfses");

            migrationBuilder.DropIndex(
                name: "IX_Ecmwfs_WindyTimeId",
                table: "Ecmwfs");

            migrationBuilder.DropColumn(
                name: "WindyTimeId",
                table: "Gfses");

            migrationBuilder.DropColumn(
                name: "WindyTimeId",
                table: "Ecmwfs");

            migrationBuilder.AddColumn<long>(
                name: "Start",
                table: "Gfses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "Step",
                table: "Gfses",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "Start",
                table: "Ecmwfs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "Step",
                table: "Ecmwfs",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
