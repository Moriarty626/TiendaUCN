using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaUCN.src.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixJwtBlacklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "JwtBlacklist",
                newName: "TokenId");

            migrationBuilder.RenameIndex(
                name: "IX_JwtBlacklist_Token",
                table: "JwtBlacklist",
                newName: "IX_JwtBlacklist_TokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenId",
                table: "JwtBlacklist",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_JwtBlacklist_TokenId",
                table: "JwtBlacklist",
                newName: "IX_JwtBlacklist_Token");
        }
    }
}
