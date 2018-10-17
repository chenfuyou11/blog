using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogDemo.Infrastructure.Migrations
{
    public partial class Addremarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "T_Posts",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                table: "T_Posts");
        }
    }
}
