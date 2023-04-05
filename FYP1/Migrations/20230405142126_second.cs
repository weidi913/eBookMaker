using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Object_Page_bookPageID",
                table: "Object");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Object",
                table: "Object");

            migrationBuilder.RenameTable(
                name: "Object",
                newName: "Element");

            migrationBuilder.RenameIndex(
                name: "IX_Object_bookPageID",
                table: "Element",
                newName: "IX_Element_bookPageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Element",
                table: "Element",
                column: "elementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Element_Page_bookPageID",
                table: "Element",
                column: "bookPageID",
                principalTable: "Page",
                principalColumn: "bookPageID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Element_Page_bookPageID",
                table: "Element");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Element",
                table: "Element");

            migrationBuilder.RenameTable(
                name: "Element",
                newName: "Object");

            migrationBuilder.RenameIndex(
                name: "IX_Element_bookPageID",
                table: "Object",
                newName: "IX_Object_bookPageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Object",
                table: "Object",
                column: "elementID");

            migrationBuilder.AddForeignKey(
                name: "FK_Object_Page_bookPageID",
                table: "Object",
                column: "bookPageID",
                principalTable: "Page",
                principalColumn: "bookPageID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
