using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class change_table_IncomingEntries_add_FK_incomingEntryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IncomingEntries_IncomingEntryTypeId",
                table: "IncomingEntries",
                column: "IncomingEntryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingEntries_IncomingEntryTypes_IncomingEntryTypeId",
                table: "IncomingEntries",
                column: "IncomingEntryTypeId",
                principalTable: "IncomingEntryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingEntries_IncomingEntryTypes_IncomingEntryTypeId",
                table: "IncomingEntries");

            migrationBuilder.DropIndex(
                name: "IX_IncomingEntries_IncomingEntryTypeId",
                table: "IncomingEntries");
        }
    }
}
