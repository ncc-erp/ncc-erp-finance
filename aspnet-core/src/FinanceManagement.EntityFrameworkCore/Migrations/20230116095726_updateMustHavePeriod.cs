using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class updateMustHavePeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "OutcomingEntrySupplier",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "OutcomingEntryStatusHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "OutcomingEntryDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "OutcomingEntryBankTransaction",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "OutcomingEntrySupplier");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "OutcomingEntryStatusHistories");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "OutcomingEntryDetails");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "OutcomingEntryBankTransaction");
        }
    }
}
