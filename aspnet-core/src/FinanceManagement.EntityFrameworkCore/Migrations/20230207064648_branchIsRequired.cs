using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class branchIsRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntries_Branches_BranchId",
                table: "OutcomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntryDetails_Branches_BranchId",
                table: "OutcomingEntryDetails");

            migrationBuilder.AlterColumn<long>(
                name: "BranchId",
                table: "OutcomingEntryDetails",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BranchId",
                table: "OutcomingEntries",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntries_Branches_BranchId",
                table: "OutcomingEntries",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntryDetails_Branches_BranchId",
                table: "OutcomingEntryDetails",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntries_Branches_BranchId",
                table: "OutcomingEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntryDetails_Branches_BranchId",
                table: "OutcomingEntryDetails");

            migrationBuilder.AlterColumn<long>(
                name: "BranchId",
                table: "OutcomingEntryDetails",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "BranchId",
                table: "OutcomingEntries",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntries_Branches_BranchId",
                table: "OutcomingEntries",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntryDetails_Branches_BranchId",
                table: "OutcomingEntryDetails",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
