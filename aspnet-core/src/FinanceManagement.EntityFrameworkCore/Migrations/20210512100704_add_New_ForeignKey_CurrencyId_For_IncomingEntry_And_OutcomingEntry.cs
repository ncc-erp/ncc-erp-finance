using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class add_New_ForeignKey_CurrencyId_For_IncomingEntry_And_OutcomingEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "OutcomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "IncomingEntries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntries_CurrencyId",
                table: "OutcomingEntries",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingEntries_CurrencyId",
                table: "IncomingEntries",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Currencies_CurrencyId",
                table: "IncomingEntries",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntries_Currencies_CurrencyId",
                table: "OutcomingEntries",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Currencies_CurrencyId",
                table: "IncomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntries_Currencies_CurrencyId",
                table: "OutcomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_OutcomingEntries_CurrencyId",
                table: "OutcomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_IncomingEntries_CurrencyId",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "OutcomingEntries");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "IncomingEntries");
        }
    }
}
