using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class add_new_table_CircleChart_and_CircleChartDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CircleCharts",
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
                    Name = table.Column<string>(maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsIncome = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircleCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircleCharts_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CircleCharts_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CircleChartDetails",
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
                    CircleChartId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true),
                    Color = table.Column<string>(nullable: true),
                    BranchId = table.Column<long>(nullable: true),
                    RevenueExpenseType = table.Column<long>(nullable: true),
                    ClientIds = table.Column<string>(nullable: true),
                    InOutcomeTypeIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircleChartDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircleChartDetails_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CircleChartDetails_CircleCharts_CircleChartId",
                        column: x => x.CircleChartId,
                        principalTable: "CircleCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CircleChartDetails_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CircleChartDetails_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CircleChartDetails_BranchId",
                table: "CircleChartDetails",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleChartDetails_CircleChartId",
                table: "CircleChartDetails",
                column: "CircleChartId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleChartDetails_CreatorUserId",
                table: "CircleChartDetails",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleChartDetails_LastModifierUserId",
                table: "CircleChartDetails",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleCharts_CreatorUserId",
                table: "CircleCharts",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleCharts_LastModifierUserId",
                table: "CircleCharts",
                column: "LastModifierUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CircleChartDetails");

            migrationBuilder.DropTable(
                name: "CircleCharts");
        }
    }
}
