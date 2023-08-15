using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceManagement.Migrations
{
    public partial class change_RoleOutComingType_To_UserOutComingType_And_Edit_FK_UserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleOutcomingTypes");

            migrationBuilder.CreateTable(
                name: "UserOutcomingType",
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
                    UserId = table.Column<long>(nullable: false),
                    OutcomingEntryTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOutcomingType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOutcomingType_OutcomingEntryTypes_OutcomingEntryTypeId",
                        column: x => x.OutcomingEntryTypeId,
                        principalTable: "OutcomingEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOutcomingType_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOutcomingType_OutcomingEntryTypeId",
                table: "UserOutcomingType",
                column: "OutcomingEntryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOutcomingType_UserId",
                table: "UserOutcomingType",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOutcomingType");

            migrationBuilder.CreateTable(
                name: "RoleOutcomingTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OutcomingEntryTypeId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOutcomingTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleOutcomingTypes_OutcomingEntryTypes_OutcomingEntryTypeId",
                        column: x => x.OutcomingEntryTypeId,
                        principalTable: "OutcomingEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleOutcomingTypes_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleOutcomingTypes_OutcomingEntryTypeId",
                table: "RoleOutcomingTypes",
                column: "OutcomingEntryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleOutcomingTypes_RoleId",
                table: "RoleOutcomingTypes",
                column: "RoleId");
        }
    }
}
