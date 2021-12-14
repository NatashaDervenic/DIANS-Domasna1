using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webSchool.Data.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numOfStudents",
                table: "schools");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "numOfStudents",
                table: "schools",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
