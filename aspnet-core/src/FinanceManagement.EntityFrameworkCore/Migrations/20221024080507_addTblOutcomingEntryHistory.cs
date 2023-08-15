using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class addTblOutcomingEntryHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultBankAccountId",
                table: "Currencies",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OutcomingEntryHistories",
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
                    OutcomingEntryId = table.Column<long>(nullable: false),
                    FromValue = table.Column<double>(nullable: false),
                    ToValue = table.Column<double>(nullable: false),
                    Note = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomingEntryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryHistories_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryHistories_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutcomingEntryHistories_OutcomingEntries_OutcomingEntryId",
                        column: x => x.OutcomingEntryId,
                        principalTable: "OutcomingEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryHistories_CreatorUserId",
                table: "OutcomingEntryHistories",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryHistories_LastModifierUserId",
                table: "OutcomingEntryHistories",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryHistories_OutcomingEntryId",
                table: "OutcomingEntryHistories",
                column: "OutcomingEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutcomingEntryHistories");

            migrationBuilder.DropColumn(
                name: "DefaultBankAccountId",
                table: "Currencies");
        }
    }
}
