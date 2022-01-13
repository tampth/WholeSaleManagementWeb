using Microsoft.EntityFrameworkCore.Migrations;

namespace WholeSaleManagementApp.Migrations
{
    public partial class AddThumb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Thumb",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumb",
                table: "Posts");
        }
    }
}
