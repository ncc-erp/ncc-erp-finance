using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class update_Table_OutcomingEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "isAcceptFile",
                table: "OutcomingEntries",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isAcceptFile",
                table: "OutcomingEntries",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte));
        }
    }
}
