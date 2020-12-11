using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class InitialMySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dimensions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Zoom = table.Column<short>(nullable: false),
                    X = table.Column<short>(nullable: false),
                    Y = table.Column<short>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dimensions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WindyTimes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true),
                    Start = table.Column<long>(nullable: false),
                    Step = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindyTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forecasts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Start = table.Column<long>(nullable: false),
                    Step = table.Column<short>(nullable: false),
                    GFSContent = table.Column<string>(nullable: true),
                    ECMWFContent = table.Column<string>(nullable: true),
                    DimensionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forecasts_Dimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "Dimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    O = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    ExtraField1 = table.Column<int>(nullable: false),
                    ExtraField2 = table.Column<int>(nullable: false),
                    FullText = table.Column<string>(nullable: true),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    DimensionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_Dimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "Dimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ecmwfs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(nullable: true),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    DataString = table.Column<string>(nullable: true),
                    DimensionId = table.Column<int>(nullable: false),
                    WindyTimeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ecmwfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ecmwfs_Dimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "Dimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ecmwfs_WindyTimes_WindyTimeId",
                        column: x => x.WindyTimeId,
                        principalTable: "WindyTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gfses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(nullable: true),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    DataString = table.Column<string>(nullable: true),
                    DimensionId = table.Column<int>(nullable: false),
                    WindyTimeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gfses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gfses_Dimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "Dimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gfses_WindyTimes_WindyTimeId",
                        column: x => x.WindyTimeId,
                        principalTable: "WindyTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ecmwfs_DimensionId",
                table: "Ecmwfs",
                column: "DimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ecmwfs_WindyTimeId",
                table: "Ecmwfs",
                column: "WindyTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Forecasts_DimensionId",
                table: "Forecasts",
                column: "DimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Gfses_DimensionId",
                table: "Gfses",
                column: "DimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Gfses_WindyTimeId",
                table: "Gfses",
                column: "WindyTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DimensionId",
                table: "Labels",
                column: "DimensionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ecmwfs");

            migrationBuilder.DropTable(
                name: "Forecasts");

            migrationBuilder.DropTable(
                name: "Gfses");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "WindyTimes");

            migrationBuilder.DropTable(
                name: "Dimensions");
        }
    }
}
