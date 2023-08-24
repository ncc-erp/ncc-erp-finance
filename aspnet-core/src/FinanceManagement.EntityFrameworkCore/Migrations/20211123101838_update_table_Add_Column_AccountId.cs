using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class update_table_Add_Column_AccountId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer",
                table: "RevenueManageds");

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "RevenueManageds",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenueManageds_AccountId",
                table: "RevenueManageds",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_RevenueManageds_Accounts_AccountId",
                table: "RevenueManageds",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RevenueManageds_Accounts_AccountId",
                table: "RevenueManageds");

            migrationBuilder.DropIndex(
                name: "IX_RevenueManageds_AccountId",
                table: "RevenueManageds");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "RevenueManageds");

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "RevenueManageds",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
