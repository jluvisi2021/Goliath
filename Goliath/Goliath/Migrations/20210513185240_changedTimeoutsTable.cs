using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class changedTimeoutsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UnauthorizedTimeoutTable",
                table: "UnauthorizedTimeoutTable");

            migrationBuilder.RenameTable(
                name: "UnauthorizedTimeoutTable",
                newName: "UserTimeoutsTable");

            migrationBuilder.AddColumn<string>(
                name: "RequestDataDownload",
                table: "UserTimeoutsTable",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestTwoFactorSmsAuthorized",
                table: "UserTimeoutsTable",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateProfileSettings",
                table: "UserTimeoutsTable",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTimeoutsTable",
                table: "UserTimeoutsTable",
                column: "NumericId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTimeoutsTable",
                table: "UserTimeoutsTable");

            migrationBuilder.DropColumn(
                name: "RequestDataDownload",
                table: "UserTimeoutsTable");

            migrationBuilder.DropColumn(
                name: "RequestTwoFactorSmsAuthorized",
                table: "UserTimeoutsTable");

            migrationBuilder.DropColumn(
                name: "UpdateProfileSettings",
                table: "UserTimeoutsTable");

            migrationBuilder.RenameTable(
                name: "UserTimeoutsTable",
                newName: "UnauthorizedTimeoutTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnauthorizedTimeoutTable",
                table: "UnauthorizedTimeoutTable",
                column: "NumericId");
        }
    }
}
