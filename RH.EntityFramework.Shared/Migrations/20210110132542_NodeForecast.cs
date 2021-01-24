using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class NodeForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forecasts");

            migrationBuilder.CreateTable(
                name: "WindDimensions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindDimensions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EcmwfForecasts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WindDimensionId = table.Column<int>(nullable: false),
                    ReferenceTime = table.Column<DateTime>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    OrigTs = table.Column<long>(nullable: false),
                    WeatherCode = table.Column<string>(nullable: true),
                    Mm = table.Column<double>(nullable: false),
                    SnowPrecip = table.Column<double>(nullable: false),
                    ConvPrecip = table.Column<double>(nullable: false),
                    Rain = table.Column<bool>(nullable: false),
                    Snow = table.Column<bool>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    DewPoint = table.Column<double>(nullable: false),
                    Wind = table.Column<double>(nullable: false),
                    WindDirection = table.Column<int>(nullable: false),
                    Rh = table.Column<int>(nullable: false),
                    Gust = table.Column<double>(nullable: false),
                    Pressure = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcmwfForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcmwfForecasts_WindDimensions_WindDimensionId",
                        column: x => x.WindDimensionId,
                        principalTable: "WindDimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GfsForecasts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WindDimensionId = table.Column<int>(nullable: false),
                    ReferenceTime = table.Column<DateTime>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    OrigTs = table.Column<long>(nullable: false),
                    WeatherCode = table.Column<string>(nullable: true),
                    Mm = table.Column<double>(nullable: false),
                    SnowPrecip = table.Column<double>(nullable: false),
                    ConvPrecip = table.Column<double>(nullable: false),
                    Rain = table.Column<bool>(nullable: false),
                    Snow = table.Column<bool>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    DewPoint = table.Column<double>(nullable: false),
                    Wind = table.Column<double>(nullable: false),
                    WindDirection = table.Column<int>(nullable: false),
                    Rh = table.Column<int>(nullable: false),
                    Gust = table.Column<double>(nullable: false),
                    Pressure = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GfsForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GfsForecasts_WindDimensions_WindDimensionId",
                        column: x => x.WindDimensionId,
                        principalTable: "WindDimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcmwfForecasts_WindDimensionId",
                table: "EcmwfForecasts",
                column: "WindDimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_GfsForecasts_WindDimensionId",
                table: "GfsForecasts",
                column: "WindDimensionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcmwfForecasts");

            migrationBuilder.DropTable(
                name: "GfsForecasts");

            migrationBuilder.DropTable(
                name: "WindDimensions");

            migrationBuilder.CreateTable(
                name: "Forecasts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DimensionId = table.Column<int>(type: "int", nullable: false),
                    ECMWFContent = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    GFSContent = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Start = table.Column<long>(type: "bigint", nullable: false),
                    Step = table.Column<short>(type: "smallint", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Forecasts_DimensionId",
                table: "Forecasts",
                column: "DimensionId");
        }
    }
}
