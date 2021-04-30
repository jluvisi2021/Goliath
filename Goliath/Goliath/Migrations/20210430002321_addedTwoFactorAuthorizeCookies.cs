using Microsoft.EntityFrameworkCore.Migrations;

namespace Goliath.Migrations
{
    public partial class addedTwoFactorAuthorizeCookies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwoFactorAuthorizationTokens",
                columns: table => new
                {
                    NumericID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    AuthorizeToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorAuthorizationTokens", x => x.NumericID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwoFactorAuthorizationTokens");
        }
    }
}