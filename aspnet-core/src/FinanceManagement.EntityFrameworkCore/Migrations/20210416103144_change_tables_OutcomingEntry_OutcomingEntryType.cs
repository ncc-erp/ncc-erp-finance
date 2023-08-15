using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class change_tables_OutcomingEntry_OutcomingEntryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WorkflowId",
                table: "OutcomingEntryTypes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WorkflowStatusId",
                table: "OutcomingEntries",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_OutcomingEntries_WorkflowStatusId",
                table: "OutcomingEntries",
                column: "WorkflowStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomingEntries_WorkflowStatuses_WorkflowStatusId",
                table: "OutcomingEntries",
                column: "WorkflowStatusId",
                principalTable: "WorkflowStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutcomingEntries_WorkflowStatuses_WorkflowStatusId",
                table: "OutcomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_OutcomingEntries_WorkflowStatusId",
                table: "OutcomingEntries");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "OutcomingEntryTypes");

            migrationBuilder.DropColumn(
                name: "WorkflowStatusId",
                table: "OutcomingEntries");
        }
    }
}
