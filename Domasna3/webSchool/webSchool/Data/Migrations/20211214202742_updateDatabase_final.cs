using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webSchool.Data.Migrations
{
    public partial class updateDatabase_final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "numOfStudents",
                table: "schools",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
