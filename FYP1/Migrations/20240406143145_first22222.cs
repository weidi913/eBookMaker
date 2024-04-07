using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1.Migrations
{
    public partial class first22222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bookContent",
                table: "eBook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bookContent",
                table: "eBook");
        }
    }
}
