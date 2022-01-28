using Microsoft.EntityFrameworkCore.Migrations;

namespace RH.EntityFramework.Shared.Migrations
{
    public partial class columnComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WindDirection",
                table: "GfsForecasts",
                type: "int",
                nullable: false,
                comment: "Degree",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Wind",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "m/s",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "SnowPrecip",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "Millimeter",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "Rh",
                table: "GfsForecasts",
                type: "int",
                nullable: false,
                comment: "Percent",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Pressure",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "Pascal",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<long>(
                name: "OrigTs",
                table: "GfsForecasts",
                type: "bigint",
                nullable: false,
                comment: "Epoch Time",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "Gust",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "m/s",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "DewPoint",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "Kelvin",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "ConvPrecip",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                comment: "Millimeter",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<string>(
                name: "DataString",
                table: "Gfses",
                type: "longtext",
                nullable: true,
                comment: "Kelvin",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DataString",
                table: "Ecmwfs",
                type: "longtext",
                nullable: true,
                comment: "Kelvin",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "WindDirection",
                table: "EcmwfForecasts",
                type: "int",
                nullable: false,
                comment: "Degree",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Wind",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "m/s",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "SnowPrecip",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "Millimeter",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "Rh",
                table: "EcmwfForecasts",
                type: "int",
                nullable: false,
                comment: "Percent",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Pressure",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "Pascal",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<long>(
                name: "OrigTs",
                table: "EcmwfForecasts",
                type: "bigint",
                nullable: false,
                comment: "Epoch Time",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "Gust",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "m/s",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "DewPoint",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "Kelvin",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "ConvPrecip",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                comment: "Millimeter",
                oldClrType: typeof(double),
                oldType: "double");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WindDirection",
                table: "GfsForecasts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Degree");

            migrationBuilder.AlterColumn<double>(
                name: "Wind",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "m/s");

            migrationBuilder.AlterColumn<double>(
                name: "SnowPrecip",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Millimeter");

            migrationBuilder.AlterColumn<int>(
                name: "Rh",
                table: "GfsForecasts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Percent");

            migrationBuilder.AlterColumn<double>(
                name: "Pressure",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Pascal");

            migrationBuilder.AlterColumn<long>(
                name: "OrigTs",
                table: "GfsForecasts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "Epoch Time");

            migrationBuilder.AlterColumn<double>(
                name: "Gust",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "m/s");

            migrationBuilder.AlterColumn<double>(
                name: "DewPoint",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Kelvin");

            migrationBuilder.AlterColumn<double>(
                name: "ConvPrecip",
                table: "GfsForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Millimeter");

            migrationBuilder.AlterColumn<string>(
                name: "DataString",
                table: "Gfses",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "Kelvin")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DataString",
                table: "Ecmwfs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "Kelvin")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "WindDirection",
                table: "EcmwfForecasts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Degree");

            migrationBuilder.AlterColumn<double>(
                name: "Wind",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "m/s");

            migrationBuilder.AlterColumn<double>(
                name: "SnowPrecip",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Millimeter");

            migrationBuilder.AlterColumn<int>(
                name: "Rh",
                table: "EcmwfForecasts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Percent");

            migrationBuilder.AlterColumn<double>(
                name: "Pressure",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Pascal");

            migrationBuilder.AlterColumn<long>(
                name: "OrigTs",
                table: "EcmwfForecasts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "Epoch Time");

            migrationBuilder.AlterColumn<double>(
                name: "Gust",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "m/s");

            migrationBuilder.AlterColumn<double>(
                name: "DewPoint",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Kelvin");

            migrationBuilder.AlterColumn<double>(
                name: "ConvPrecip",
                table: "EcmwfForecasts",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Millimeter");
        }
    }
}
