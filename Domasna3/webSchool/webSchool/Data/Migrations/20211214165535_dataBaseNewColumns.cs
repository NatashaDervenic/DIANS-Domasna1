using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webSchool.Data.Migrations
{
    public partial class dataBaseNewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "contact",
                table: "schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "schools",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "schools",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "teachers",
                table: "schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "workTime",
                table: "schools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "email",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "teachers",
                table: "schools");

            migrationBuilder.DropColumn(
                name: "workTime",
                table: "schools");
        }
    }
}
