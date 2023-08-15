using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class add_table_OutcomingEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutcomingEntries",
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
                    AccountId = table.Column<long>(nullable: false),
                    BranchId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomingEntries_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomingEntries_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutcomingEntries_OutcomingEntryTypes_OutcomingEntryTypeId",
                        column: x => x.OutcomingEntryTypeId,
                        principalTable: "OutcomingEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntries_AccountId",
                table: "OutcomingEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntries_BranchId",
                table: "OutcomingEntries",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntries_OutcomingEntryTypeId",
                table: "OutcomingEntries",
                column: "OutcomingEntryTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutcomingEntries");
        }
    }
}
