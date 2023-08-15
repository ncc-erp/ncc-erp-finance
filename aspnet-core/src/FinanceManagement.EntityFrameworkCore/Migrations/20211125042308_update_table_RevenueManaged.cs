using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class update_table_RevenueManaged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingDebt",
                table: "RevenueManageds");

            migrationBuilder.AlterColumn<double>(
                name: "CollectionDebt",
                table: "RevenueManageds",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<double>(
                name: "DebtReceived",
                table: "RevenueManageds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtReceived",
                table: "RevenueManageds");

            migrationBuilder.AlterColumn<long>(
                name: "CollectionDebt",
                table: "RevenueManageds",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<long>(
                name: "RemainingDebt",
                table: "RevenueManageds",
                type: "bigint",
                nullable: true);
        }
    }
}
