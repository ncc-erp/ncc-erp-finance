using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class updateTblRevenueManaged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "RevenueManageds");

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "RevenueManageds",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RevenueManageds_UnitId",
                table: "RevenueManageds",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_RevenueManageds_Currencies_UnitId",
                table: "RevenueManageds",
                column: "UnitId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RevenueManageds_Currencies_UnitId",
                table: "RevenueManageds");

            migrationBuilder.DropIndex(
                name: "IX_RevenueManageds_UnitId",
                table: "RevenueManageds");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "RevenueManageds");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "RevenueManageds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
