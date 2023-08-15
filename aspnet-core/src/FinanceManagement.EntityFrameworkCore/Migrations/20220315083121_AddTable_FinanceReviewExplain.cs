using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class AddTable_FinanceReviewExplain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceReviewExplains",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IncomingVND = table.Column<double>(nullable: false),
                    IncomingUSD = table.Column<double>(nullable: false),
                    IncomingVNDTransaction = table.Column<double>(nullable: false),
                    IncomingUSDTransaction = table.Column<double>(nullable: false),
                    OutcomingVND = table.Column<double>(nullable: false),
                    OutcomingUSD = table.Column<double>(nullable: false),
                    OutcomingVNDTransaction = table.Column<double>(nullable: false),
                    OutcomingUSDTransaction = table.Column<double>(nullable: false),
                    IncomingDiffVND = table.Column<double>(nullable: false),
                    IncomingDiffUSD = table.Column<double>(nullable: false),
                    OutcomingDiffVND = table.Column<double>(nullable: false),
                    OutcomingDiffUSD = table.Column<double>(nullable: false),
                    IncomingDiffVNDNote = table.Column<string>(nullable: true),
                    IncomingDiffUSDNote = table.Column<string>(nullable: true),
                    OutcomingDiffVNDNote = table.Column<string>(nullable: true),
                    OutcomingDiffUSDNote = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceReviewExplains", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanceReviewExplains");
        }
    }
}
