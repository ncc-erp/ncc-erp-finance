using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class addFieldBranchId_Key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BTransactionLogs_Message",
                table: "BTransactionLogs");

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "OutcomingEntryDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "BTransactionLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntryDetails_BranchId",
                table: "OutcomingEntryDetails",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntryDetails_Branches_BranchId",
                table: "OutcomingEntryDetails",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntryDetails_Branches_BranchId",
                table: "OutcomingEntryDetails");

            migrationBuilder.DropIndex(
                name: "IX_OutcomingEntryDetails_BranchId",
                table: "OutcomingEntryDetails");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "OutcomingEntryDetails");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "BTransactionLogs");

            migrationBuilder.CreateIndex(
                name: "IX_BTransactionLogs_Message",
                table: "BTransactionLogs",
                column: "Message",
                unique: true,
                filter: "[Message] IS NOT NULL");
        }
    }
}
