using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class addedMoreUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountCreationDate",
                table: "AspNetUsers",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountLoginHistory",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastPasswordUpdate",
                table: "AspNetUsers",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUserLogin",
                table: "AspNetUsers",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnverifiedNewEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCreationDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AccountLoginHistory",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastPasswordUpdate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUserLogin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UnverifiedNewEmail",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}