using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class update_Table_WorkflowStatus_And_WorklowStatusTransition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStatuses_Workflows_WorkflowId",
                table: "WorkflowStatuses");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowStatuses_WorkflowId",
                table: "WorkflowStatuses");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "WorkflowStatuses");

            migrationBuilder.AddColumn<long>(
                name: "WorkflowId",
                table: "WorkflowStatusTransitions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStatusTransitions_WorkflowId",
                table: "WorkflowStatusTransitions",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStatusTransitions_Workflows_WorkflowId",
                table: "WorkflowStatusTransitions",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id");
                //onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStatusTransitions_Workflows_WorkflowId",
                table: "WorkflowStatusTransitions");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowStatusTransitions_WorkflowId",
                table: "WorkflowStatusTransitions");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "WorkflowStatusTransitions");

            migrationBuilder.AddColumn<long>(
                name: "WorkflowId",
                table: "WorkflowStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStatuses_WorkflowId",
                table: "WorkflowStatuses",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStatuses_Workflows_WorkflowId",
                table: "WorkflowStatuses",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
