using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class addedMaxLengthToCaptchaModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GeneratedDateTime",
                table: "ValidCaptchaCookieTokens",
                type: "nvarchar(22)",
                maxLength: 22,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GeneratedDateTime",
                table: "ValidCaptchaCookieTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(22)",
                oldMaxLength: 22);
        }
    }
}