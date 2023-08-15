using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class update_Table_BankTransaction_And_Create_New_Table_Invoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "BankTransactions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PayForInvoice",
                table: "BankTransactions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TimeAt = table.Column<string>(nullable: true),
                    AccountCode = table.Column<string>(nullable: true),
                    Project = table.Column<string>(nullable: true),
                    TotalPrice = table.Column<double>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreatedBy = table.Column<byte>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_Invoices_InvoiceId",
                table: "BankTransactions");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_InvoiceId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "PayForInvoice",
                table: "BankTransactions");
        }
    }
}
