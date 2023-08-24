using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class Add_Column_IsActive_To_Table_IncomingEntryType_OutcomingEntryType_BankAccount_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OutcomingEntryTypes",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "IncomingEntryTypes",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BankAccounts",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OutcomingEntryTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "IncomingEntryTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accounts");
        }
    }
}
