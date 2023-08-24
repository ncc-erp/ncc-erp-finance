using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class add_table_OutcomingEntryBankTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutcomingEntryBankTransaction",
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
                    BankTransactionId = table.Column<long>(nullable: false),
                    OutcomingEntryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomingEntryBankTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryBankTransaction_BankTransactions_BankTransactionId",
                        column: x => x.BankTransactionId,
                        principalTable: "BankTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryBankTransaction_OutcomingEntries_OutcomingEntryId",
                        column: x => x.OutcomingEntryId,
                        principalTable: "OutcomingEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryBankTransaction_BankTransactionId",
                table: "OutcomingEntryBankTransaction",
                column: "BankTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryBankTransaction_OutcomingEntryId",
                table: "OutcomingEntryBankTransaction",
                column: "OutcomingEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutcomingEntryBankTransaction");
        }
    }
}
