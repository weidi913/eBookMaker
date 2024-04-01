using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BookPage_bookPageID",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "bookPageID",
                table: "Comment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "bookID",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                columns: new[] { "authorID", "bookID" });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_bookID",
                table: "Comment",
                column: "bookID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BookPage_bookPageID",
                table: "Comment",
                column: "bookPageID",
                principalTable: "BookPage",
                principalColumn: "bookPageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_eBook_bookID",
                table: "Comment",
                column: "bookID",
                principalTable: "eBook",
                principalColumn: "bookID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BookPage_bookPageID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_eBook_bookID",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_bookID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "bookID",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "bookPageID",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                columns: new[] { "authorID", "bookPageID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BookPage_bookPageID",
                table: "Comment",
                column: "bookPageID",
                principalTable: "BookPage",
                principalColumn: "bookPageID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
