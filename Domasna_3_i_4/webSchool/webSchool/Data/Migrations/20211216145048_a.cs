using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webSchool.Data.Migrations
{
    public partial class a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
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
