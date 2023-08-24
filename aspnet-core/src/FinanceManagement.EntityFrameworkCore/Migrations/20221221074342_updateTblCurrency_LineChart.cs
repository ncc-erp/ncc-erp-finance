using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class updateTblCurrency_LineChart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotDone",
                table: "OutcomingEntryDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAt",
                table: "CurrencyConverts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "DefaultBankAccountIdWhenSell",
                table: "Currencies",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultFromBankAccountIdWhenBuy",
                table: "Currencies",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultToBankAccountIdWhenBuy",
                table: "Currencies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineChartSettings_LinechartId",
                table: "LineChartSettings",
                column: "LinechartId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineChartSettings_LineCharts_LinechartId",
                table: "LineChartSettings",
                column: "LinechartId",
                principalTable: "LineCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineChartSettings_LineCharts_LinechartId",
                table: "LineChartSettings");

            migrationBuilder.DropIndex(
                name: "IX_LineChartSettings_LinechartId",
                table: "LineChartSettings");

            migrationBuilder.DropColumn(
                name: "IsNotDone",
                table: "OutcomingEntryDetails");

            migrationBuilder.DropColumn(
                name: "DateAt",
                table: "CurrencyConverts");

            migrationBuilder.DropColumn(
                name: "DefaultBankAccountIdWhenSell",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "DefaultFromBankAccountIdWhenBuy",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "DefaultToBankAccountIdWhenBuy",
                table: "Currencies");
        }
    }
}
