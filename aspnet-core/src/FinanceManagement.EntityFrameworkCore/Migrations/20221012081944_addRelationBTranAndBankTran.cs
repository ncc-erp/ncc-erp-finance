using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class addRelationBTranAndBankTran : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BTransactionId",
                table: "BankTransactions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_BTransactionId",
                table: "BankTransactions",
                column: "BTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactions_BTransactions_BTransactionId",
                table: "BankTransactions",
                column: "BTransactionId",
                principalTable: "BTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_BTransactions_BTransactionId",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_BTransactionId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "BTransactionId",
                table: "BankTransactions");
        }
    }
}
