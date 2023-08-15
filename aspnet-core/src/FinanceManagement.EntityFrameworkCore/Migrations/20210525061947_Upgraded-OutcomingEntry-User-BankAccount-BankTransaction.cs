using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class UpgradedOutcomingEntryUserBankAccountBankTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedTime",
                table: "OutcomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutedTime",
                table: "OutcomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentTime",
                table: "OutcomingEntries",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LockedStatus",
                table: "BankTransactions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockedStatus",
                table: "BankAccounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KomuUserName",
                table: "AbpUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedTime",
                table: "OutcomingEntries");

            migrationBuilder.DropColumn(
                name: "ExecutedTime",
                table: "OutcomingEntries");

            migrationBuilder.DropColumn(
                name: "SentTime",
                table: "OutcomingEntries");

            migrationBuilder.DropColumn(
                name: "LockedStatus",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "LockedStatus",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "KomuUserName",
                table: "AbpUsers");
        }
    }
}
