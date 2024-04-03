using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1.Migrations
{
    public partial class bg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "background",
                table: "eBook");

            migrationBuilder.AddColumn<string>(
                name: "backgroundStyle",
                table: "BookPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "backgroundStyle",
                table: "BookPage");

            migrationBuilder.AddColumn<string>(
                name: "background",
                table: "eBook",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
