using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class addedAccountNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PendingNotifications",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingNotifications",
                table: "AspNetUsers");
        }
    }
}
