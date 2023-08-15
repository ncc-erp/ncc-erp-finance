using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class addTempOutcoming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutcomingEntryMoneyHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "BTransactionLogs",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TempOutcomingEntries",
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
                    OutcomingEntryTypeId = table.Column<long>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    WorkflowStatusId = table.Column<long>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    BranchId = table.Column<long>(nullable: true),
                    CurrencyId = table.Column<long>(nullable: true),
                    SentTime = table.Column<DateTime>(nullable: true),
                    ApprovedTime = table.Column<DateTime>(nullable: true),
                    ExecutedTime = table.Column<DateTime>(nullable: true),
                    PaymentCode = table.Column<string>(nullable: true),
                    Accreditation = table.Column<bool>(nullable: false),
                    IsAcceptFile = table.Column<byte>(nullable: false),
                    IsOriginal = table.Column<bool>(nullable: false),
                    RootOutcomingEntryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempOutcomingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempOutcomingEntries_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempOutcomingEntries_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempOutcomingEntries_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempOutcomingEntries_OutcomingEntryTypes_OutcomingEntryTypeId",
                        column: x => x.OutcomingEntryTypeId,
                        principalTable: "OutcomingEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempOutcomingEntries_WorkflowStatuses_WorkflowStatusId",
                        column: x => x.WorkflowStatusId,
                        principalTable: "WorkflowStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempOutcomingEntryDetails",
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
                    AccountId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<double>(nullable: false),
                    UnitPrice = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    OutcomingEntryId = table.Column<long>(nullable: false),
                    BranchId = table.Column<long>(nullable: true),
                    RootOutcomingEntryDetailId = table.Column<long>(nullable: true),
                    RootTempOutcomingEntryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempOutcomingEntryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempOutcomingEntryDetails_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BTransactionLogs_Key",
                table: "BTransactionLogs",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_TempOutcomingEntries_AccountId",
                table: "TempOutcomingEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TempOutcomingEntries_BranchId",
                table: "TempOutcomingEntries",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TempOutcomingEntries_CurrencyId",
                table: "TempOutcomingEntries",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TempOutcomingEntries_OutcomingEntryTypeId",
                table: "TempOutcomingEntries",
                column: "OutcomingEntryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TempOutcomingEntries_WorkflowStatusId",
                table: "TempOutcomingEntries",
                column: "WorkflowStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TempOutcomingEntryDetails_BranchId",
                table: "TempOutcomingEntryDetails",
                column: "BranchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempOutcomingEntries");

            migrationBuilder.DropTable(
                name: "TempOutcomingEntryDetails");

            migrationBuilder.DropIndex(
                name: "IX_BTransactionLogs_Key",
                table: "BTransactionLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "BTransactionLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OutcomingEntryMoneyHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FromValue = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OutcomingEntryId = table.Column<long>(type: "bigint", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    ToValue = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomingEntryMoneyHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryMoneyHistories_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryMoneyHistories_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryMoneyHistories_OutcomingEntries_OutcomingEntryId",
                        column: x => x.OutcomingEntryId,
                        principalTable: "OutcomingEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryMoneyHistories_CreatorUserId",
                table: "OutcomingEntryMoneyHistories",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryMoneyHistories_LastModifierUserId",
                table: "OutcomingEntryMoneyHistories",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryMoneyHistories_OutcomingEntryId",
                table: "OutcomingEntryMoneyHistories",
                column: "OutcomingEntryId");
        }
    }
}
