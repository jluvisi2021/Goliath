using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class addedLast2FAUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwoFactorUpdated",
                table: "AspNetUsers",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorUpdated",
                table: "AspNetUsers");
        }
    }
}