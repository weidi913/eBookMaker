using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1.Migrations
{
    public partial class first3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "Comment",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "commentID",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "commentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "commentID",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "comment",
                table: "Comment",
                type: "int",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                columns: new[] { "authorID", "bookID" });
        }
    }
}
