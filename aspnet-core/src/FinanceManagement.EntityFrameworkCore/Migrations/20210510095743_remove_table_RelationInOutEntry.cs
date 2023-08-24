using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class remove_table_RelationInOutEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelationInOutEntry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelationInOutEntry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncomingEntryId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OutcomingEntryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationInOutEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationInOutEntry_IncomingEntryTypes_IncomingEntryId",
                        column: x => x.IncomingEntryId,
                        principalTable: "IncomingEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelationInOutEntry_OutcomingEntryTypes_OutcomingEntryId",
                        column: x => x.OutcomingEntryId,
                        principalTable: "OutcomingEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelationInOutEntry_IncomingEntryId",
                table: "RelationInOutEntry",
                column: "IncomingEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationInOutEntry_OutcomingEntryId",
                table: "RelationInOutEntry",
                column: "OutcomingEntryId");
        }
    }
}
