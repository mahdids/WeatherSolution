using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class AddForecastType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ecmwfs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<long>(nullable: false),
                    Step = table.Column<short>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    DataString = table.Column<string>(nullable: true),
                    DimensionId = table.Column<int>(nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Gfses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<long>(nullable: false),
                    Step = table.Column<short>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    DataString = table.Column<string>(nullable: true),
                    DimensionId = table.Column<int>(nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ecmwfs_DimensionId",
                table: "Ecmwfs",
                column: "DimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_Gfses_DimensionId",
                table: "Gfses",
                column: "DimensionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ecmwfs");

            migrationBuilder.DropTable(
                name: "Gfses");
        }
    }
}
