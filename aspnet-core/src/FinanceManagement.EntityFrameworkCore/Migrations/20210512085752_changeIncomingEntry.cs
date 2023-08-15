using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class changeIncomingEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Branches_BranchId",
                table: "IncomingEntries");

            migrationBuilder.AlterColumn<long>(
                name: "BranchId",
                table: "IncomingEntries",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Branches_BranchId",
                table: "IncomingEntries",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_Branches_BranchId",
                table: "IncomingEntries");

            migrationBuilder.AlterColumn<long>(
                name: "BranchId",
                table: "IncomingEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_Branches_BranchId",
                table: "IncomingEntries",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
