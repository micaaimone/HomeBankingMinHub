using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBankingMinHub.Migrations
{
    /// <inheritdoc />
    public partial class Accounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "ClientLoans",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientLoans_AccountId",
                table: "ClientLoans",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLoans_Accounts_AccountId",
                table: "ClientLoans",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientLoans_Accounts_AccountId",
                table: "ClientLoans");

            migrationBuilder.DropIndex(
                name: "IX_ClientLoans_AccountId",
                table: "ClientLoans");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "ClientLoans");
        }
    }
}
