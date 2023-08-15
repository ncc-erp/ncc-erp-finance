using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class Upgrade_table_OutcomingEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isAcceptFile",
                table: "OutcomingEntries",
                newName: "IsAcceptFile");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAcceptFile",
                table: "OutcomingEntries",
                newName: "isAcceptFile");
        }
    }
}
