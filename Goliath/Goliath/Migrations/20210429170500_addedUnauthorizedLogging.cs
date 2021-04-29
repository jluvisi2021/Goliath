using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class addedUnauthorizedLogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "ResendSmsConfirmationToken",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "UnauthorizedTimeoutTable",
                columns: table => new
                {
                    NumericID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    RequestVerifyEmail = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    RequestForgotPassword = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    RequestForgotUsername = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    RequestTwoFactorSmsInital = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    RequestTwoFactorSmsResend = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnauthorizedTimeoutTable", x => x.NumericID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnauthorizedTimeoutTable");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "ResendSmsConfirmationToken",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36);
        }
    }
}
