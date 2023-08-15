using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class addInvoiceBankTransactioneditBankTransactioneditInvoiceDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_Invoices_InvoiceId",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_InvoiceId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "LinkFile",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "PayForInvoice",
                table: "BankTransactions");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "InvoiceDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "InvoiceDetails");

            migrationBuilder.AddColumn<string>(
                name: "LinkFile",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "BankTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PayForInvoice",
                table: "BankTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_InvoiceId",
                table: "BankTransactions",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactions_Invoices_InvoiceId",
                table: "BankTransactions",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
