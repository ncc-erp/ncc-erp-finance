using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class updateDBNewVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Accounts_AccountId",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Project",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TimeAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Invoices");

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "CollectionDebt",
                table: "Invoices",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "ITF",
                table: "Invoices",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Invoices",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Month",
                table: "Invoices",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<double>(
                name: "NTF",
                table: "Invoices",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "NameInvoice",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "BankTransactionId",
                table: "IncomingEntries",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "IncomingEntries",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "BTransactionId",
                table: "IncomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExchangeRate",
                table: "IncomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "IncomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ToCurrencyId",
                table: "IncomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MaxITF",
                table: "Currencies",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Accounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BTransactionLogs",
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
                    Message = table.Column<string>(maxLength: 3000, nullable: true),
                    TimeAt = table.Column<DateTime>(nullable: false),
                    ErrorMessage = table.Column<string>(maxLength: 2000, nullable: true),
                    BTransactionId = table.Column<long>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BTransactionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BTransactions",
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
                    BankAccountId = table.Column<long>(nullable: false),
                    Money = table.Column<double>(nullable: false),
                    TimeAt = table.Column<DateTime>(nullable: false),
                    FromAccountId = table.Column<long>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    IsCrawl = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BTransactions_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BTransactions_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BTransactions_Accounts_FromAccountId",
                        column: x => x.FromAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BTransactions_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AccountId",
                table: "Invoices",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CurrencyId",
                table: "Invoices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingEntries_BTransactionId",
                table: "IncomingEntries",
                column: "BTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingEntries_BankTransactionId",
                table: "IncomingEntries",
                column: "BankTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingEntries_InvoiceId",
                table: "IncomingEntries",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingEntries_ToCurrencyId",
                table: "IncomingEntries",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BTransactionLogs_Message",
                table: "BTransactionLogs",
                column: "Message",
                unique: true,
                filter: "[Message] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BTransactions_BankAccountId",
                table: "BTransactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BTransactions_CreatorUserId",
                table: "BTransactions",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BTransactions_FromAccountId",
                table: "BTransactions",
                column: "FromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BTransactions_LastModifierUserId",
                table: "BTransactions",
                column: "LastModifierUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Accounts_AccountId",
                table: "IncomingEntries",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_BTransactions_BTransactionId",
                table: "IncomingEntries",
                column: "BTransactionId",
                principalTable: "BTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_BankTransactions_BankTransactionId",
                table: "IncomingEntries",
                column: "BankTransactionId",
                principalTable: "BankTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Invoices_InvoiceId",
                table: "IncomingEntries",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Currencies_ToCurrencyId",
                table: "IncomingEntries",
                column: "ToCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Accounts_AccountId",
                table: "Invoices",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Currencies_CurrencyId",
                table: "Invoices",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Accounts_AccountId",
                table: "IncomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_BTransactions_BTransactionId",
                table: "IncomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_BankTransactions_BankTransactionId",
                table: "IncomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Invoices_InvoiceId",
                table: "IncomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Currencies_ToCurrencyId",
                table: "IncomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Accounts_AccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Currencies_CurrencyId",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "BTransactionLogs");

            migrationBuilder.DropTable(
                name: "BTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_AccountId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CurrencyId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_IncomingEntries_BTransactionId",
                table: "IncomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_IncomingEntries_BankTransactionId",
                table: "IncomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_IncomingEntries_InvoiceId",
                table: "IncomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_IncomingEntries_ToCurrencyId",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CollectionDebt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ITF",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NTF",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NameInvoice",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BTransactionId",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "ToCurrencyId",
                table: "IncomingEntries");

            migrationBuilder.DropColumn(
                name: "MaxITF",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CreatedBy",
                table: "Invoices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Project",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeAt",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Invoices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<long>(
                name: "BankTransactionId",
                table: "IncomingEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "IncomingEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Accounts_AccountId",
                table: "IncomingEntries",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
